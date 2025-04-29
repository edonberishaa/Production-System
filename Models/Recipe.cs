namespace CakeProduction.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public int ProductId { get; set; }
        public string? Instructions { get; set; }
        public int PreparationTime { get; set; }
        public int YieldQuantity { get; set; }
        public string? YieldUnit { get; set; }
        public Product Product { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
