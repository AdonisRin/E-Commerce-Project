using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class DiscountCode
    {
        [Key]
        public int DiscountCodeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "Fixed"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; } // e.g. 10 for 10% or 20 for 20 LEI

        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMonths(1);

        public int MaxUses { get; set; } = 100;

        public int TimesUsed { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public bool IsValid => IsActive && DateTime.Now <= ExpiryDate && TimesUsed < MaxUses;
    }
}

