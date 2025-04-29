using System.ComponentModel.DataAnnotations;

namespace CakeProduction.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public Recipe Recipe { get; set; }
    }
}
