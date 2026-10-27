using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [StringLength(50)]
        public string TargetCategory { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment is too long.")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? AdminReply { get; set; }
        public DateTime? ReplyDate { get; set; }

        [ForeignKey("MemberId")]
        public Members? Member { get; set; }
    }
}