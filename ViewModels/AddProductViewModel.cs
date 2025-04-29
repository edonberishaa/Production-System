using System.ComponentModel.DataAnnotations;


namespace CakeProduction.ViewModels
{
    public class AddProductViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Preparation Time (minutes)")]
        [Range(1, 1000)]
        public int PreparationTime { get; set; }

        [Required]
        [Display(Name = "Yield Quantity")]
        [Range(1, 100)]
        public int YieldQuantity { get; set; }

        [Required]
        [Display(Name = "Yield Unit")]
        public string YieldUnit { get; set; } = "cake";

        [Required]
        [Display(Name = "Instructions")]
        public string Instructions { get; set; }

        public List<IngredientViewModel> Ingredients { get; set; } = new();
    }
}