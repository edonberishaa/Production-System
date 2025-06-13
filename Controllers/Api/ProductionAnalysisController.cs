using CakeProduction.Data;
using CakeProduction.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Controllers.Api
{
    [Authorize]
    [Route("api/production")]
    [ApiController]
    public class ProductionAnalysisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductionAnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("analysis")]
        public async Task<IActionResult> GetAnalysis(int? productId, string? date)
        {
            // Parse the date only if it's provided
            DateTime? targetDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                    return BadRequest("Invalid date format.");
                targetDate = parsedDate;
            }

            // Start querying
            var query = _context.ProductionLogs.AsQueryable();

            // Apply filters if needed
            if (productId.HasValue)
                query = query.Where(p => p.ProductId == productId);

            if (targetDate.HasValue)
                query = query.Where(p => p.ProductionDate.Date == targetDate.Value.Date);

            // Get daily data
            var dailyData = await query.ToListAsync();

            // Get 7-day trend data if date is provided
            List<ProductionLog> trendData = new();
            if (targetDate.HasValue)
            {
                var trendQuery = _context.ProductionLogs.AsQueryable();

                if (productId.HasValue)
                    trendQuery = trendQuery.Where(p => p.ProductId == productId);

                trendData = await trendQuery
                    .Where(p => p.ProductionDate >= targetDate.Value.AddDays(-7))
                    .OrderBy(p => p.ProductionDate)
                    .ToListAsync();
            }

            // Fake timeData as 60 min per entry (can adjust later)
            var timeData = dailyData.Select(d => 60m).ToArray();

            var totalProduced = dailyData.Sum(d => d.QuantityProduced);
            var totalWasted = totalProduced * 0.05m; // 5% waste (adjust if needed)

            return Ok(new
            {
                timeLabels = dailyData.Select(d => d.ProductionDate.ToString("HH:mm")).ToArray(),
                timeData = timeData,

                totalProduced = totalProduced,
                totalWasted = totalWasted,

                trendLabels = trendData.Select(d => d.ProductionDate.ToString("MM-dd")).ToArray(),
                trendData = trendData.Select(d => d.QuantityProduced).ToArray()
            });
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel(int? productId, DateTime? fromDate, DateTime? toDate)
        {
            var logs = await _context.ProductionLogs
                .Include(p => p.Product)
                .Include(p => p.Recipe)
                .Where(p =>
                    (!productId.HasValue || p.ProductId == productId) &&
                    (!fromDate.HasValue || p.ProductionDate >= fromDate) &&
                    (!toDate.HasValue || p.ProductionDate <= toDate))
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Production History");

            // Header row
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Product";
            worksheet.Cell(1, 3).Value = "Quantity";
            worksheet.Cell(1, 4).Value = "Produced By";
            worksheet.Cell(1, 5).Value = "Notes";

            // Data rows
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                worksheet.Cell(i + 2, 1).Value = log.ProductionDate.ToString("g");
                worksheet.Cell(i + 2, 2).Value = log.Product?.Name;
                worksheet.Cell(i + 2, 3).Value = $"{log.QuantityProduced} {log.Recipe?.YieldUnit}";
                worksheet.Cell(i + 2, 4).Value = log.ProducedBy;
                worksheet.Cell(i + 2, 5).Value = log.Notes;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ProductionHistory_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

    }
}
