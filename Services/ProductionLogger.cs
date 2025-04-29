using CakeProduction.Data;
using CakeProduction.Models;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Services
{
    public class ProductionLogger :IProductionLogger
    {
        private readonly ApplicationDbContext _context;
        public ProductionLogger(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProductionLog> LogProductionAsync(int recipeId,int productId,decimal quantity,string producedBy,string notes = null)
        {
            var logEntry = new ProductionLog
            {
                RecipeId = recipeId,
                ProductId = productId,
                QuantityProduced = quantity,
                ProducedBy = producedBy,
                Notes = notes,
                ProductionDate = DateTime.Now
            };
            _context.ProductionLogs.Add(logEntry);
            await _context.SaveChangesAsync();

            return logEntry;
        }
        public async Task<ProductionLog[]> GetProductionHistoryAsync(int? productId=null,DateTime? fromDate = null,DateTime? toDate = null)
        {
            var query = _context.ProductionLogs
                .Include(pl => pl.Product)
                .Include(pl => pl.Recipe)
                .AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(pl => pl.ProductId == productId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(pl => pl.ProductionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(pl => pl.ProductionDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(pl => pl.ProductionDate)
                .ToArrayAsync();
        }
    }
}
