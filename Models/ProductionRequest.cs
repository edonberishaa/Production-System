namespace CakeProduction.Models
{
    public class ProductionRequest
    {
        public int RecipeId { get; set; }
        public decimal DesiredQuantity { get; set; }
        public string? Notes { get; set; }
    }
}
