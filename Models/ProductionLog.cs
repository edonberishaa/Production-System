using System;
using System.ComponentModel.DataAnnotations;

namespace CakeProduction.Models
{
    public class ProductionLog
    {
        [Key]
        public int ProductionLogId { get; set; }
        [Required]
        public int RecipeId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal QuantityProduced { get; set; }
        [Required]
        public DateTime ProductionDate { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(256)]
        public string ProducedBy { get; set; } 
        public string Notes { get; set; }
        public Recipe Recipe { get; set; }
        public Product Product { get; set; }
    }
}