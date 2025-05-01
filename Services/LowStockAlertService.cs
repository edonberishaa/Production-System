using CakeProduction.Data;
using CakeProduction.Models;
using CakeProduction.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

public class LowStockAlertService : ILowStockAlertService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<LowStockAlertService> _logger;

    public LowStockAlertService(
        ApplicationDbContext context,
        IEmailSender emailSender,
        ILogger<LowStockAlertService> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task CheckLowStockLevels()
    {
        try
        {
            var lowStockItems = await _context.Ingredients
                .Where(i => i.MinimumStockLevel.HasValue &&
                            i.CurrentStock < i.MinimumStockLevel)
                .ToListAsync();

            if (lowStockItems.Any())
            {
                await SendLowStockNotifications(lowStockItems);
                _logger.LogInformation($"Sent low stock alerts for {lowStockItems.Count} ingredients");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking low stock levels");
        }
    }

    public async Task SendLowStockNotifications(List<Ingredient> lowStockItems)
    {
        var subject = "Low Stock Alert - Immediate Attention Required";

        var message = new StringBuilder();
        message.AppendLine("<h2>Low Stock Alert</h2>");
        message.AppendLine("<p>The following ingredients are below their minimum stock levels:</p>");
        message.AppendLine("<table border='1' cellpadding='5'>");
        message.AppendLine("<tr><th>Ingredient</th><th>Current Stock</th><th>Minimum Level</th><th>Unit</th></tr>");

        foreach (var item in lowStockItems)
        {
            message.AppendLine($"<tr>" +
                $"<td>{item.Name}</td>" +
                $"<td>{item.CurrentStock}</td>" +
                $"<td>{item.MinimumStockLevel}</td>" +
                $"<td>{item.UnitOfMeasure}</td>" +
                $"</tr>");
        }

        message.AppendLine("</table>");
        message.AppendLine("<p>Please restock these items as soon as possible.</p>");

        await _emailSender.SendEmailAsync(
            "edonberisha52@gmail.com",
            subject,
            message.ToString());
    }
}