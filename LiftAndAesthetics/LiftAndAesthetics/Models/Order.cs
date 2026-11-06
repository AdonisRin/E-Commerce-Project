using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftAndAesthetics.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(30)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Members? Member { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DeliveryCity { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DeliveryMethod { get; set; } = "Standard Courier";

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryCost { get; set; } = 15.00m;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Card Sandbox";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0m;

        [StringLength(50)]
        public string? DiscountCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "New"; // New -> Paid -> Shipped -> Delivered, or Cancelled

        public DateTime? PaymentConfirmedAt { get; set; }

        public string? TrackingNumber { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

