using CakeProduction.Models;

namespace CakeProduction.Services
{
    public interface ILowStockAlertService
    {
        Task CheckLowStockLevels();
        Task SendLowStockNotifications(List<Ingredient> lowStockItems);
    }
}
