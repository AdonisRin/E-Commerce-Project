using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class NutritionPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "The title is mandatory")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Category { get; set; } = "General"; 

        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; } 

        public string? Ingredients { get; set; } 
        public string? Instructions { get; set; } 

        public int Calories { get; set; }
        public string? Macros { get; set; } 
    }
}