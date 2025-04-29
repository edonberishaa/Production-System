using System.ComponentModel.DataAnnotations;

namespace CakeProduction.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        public Recipe Recipe { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
