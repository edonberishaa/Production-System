using CakeProduction.Models;

namespace CakeProduction.Services
{
    public interface IProductionLogger
    {
        Task<ProductionLog> LogProductionAsync(int recipeId, int productId, decimal quantity, string producedBy, string notes = null);
        Task<ProductionLog[]> GetProductionHistoryAsync(int? productId = null, DateTime? fromDate = null, DateTime? toDate = null);

    }
}
