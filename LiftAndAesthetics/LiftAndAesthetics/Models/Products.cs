using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string Category { get; set; } = "Supplements"; 
    }
}