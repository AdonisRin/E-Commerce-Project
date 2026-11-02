using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int StockQuantity { get; set; } = 25;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsAvailable => StockQuantity > 0;
    }
}