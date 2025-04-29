using System.ComponentModel.DataAnnotations;

namespace CakeProduction.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(20)]
        public string? UnitOfMeasure { get; set; }
        [Range(0, double.MaxValue)]
        public decimal CurrentStock { get; set; }
        public decimal? MinimumStockLevel { get; set; }
        public string? SupplierInfo { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
