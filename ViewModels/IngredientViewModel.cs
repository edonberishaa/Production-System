using System.ComponentModel.DataAnnotations;

namespace CakeProduction.ViewModels
{
    public class IngredientViewModel
    {
        public int? IngredientId { get; set; }

        [Required]
        [Display(Name = "Ingredient")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [Range(0.01, 1000)]
        public decimal Quantity { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public string Unit { get; set; }
    }
}
