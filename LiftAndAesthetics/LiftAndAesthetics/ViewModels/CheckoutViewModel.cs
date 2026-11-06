using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your delivery address.")]
        [Display(Name = "Delivery Street & Number")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your city.")]
        [Display(Name = "City")]
        public string DeliveryCity { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Delivery Method")]
        public string DeliveryMethod { get; set; } = "Standard Courier"; // "Standard Courier" (15.00) or "Express Delivery" (25.00)

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Card Sandbox"; // "Card Sandbox" or "Wallet"

        [Display(Name = "Promo / Discount Code")]
        public string? DiscountCode { get; set; }

        // Test Card fields for Sandbox Payment
        [Display(Name = "Card Number")]
        public string? CardNumber { get; set; } = "4242 •••• •••• 4242";

        [Display(Name = "Expiration (MM/YY)")]
        public string? CardExpiry { get; set; } = "12/28";

        [Display(Name = "CVC / CVV")]
        public string? CardCvc { get; set; } = "123";

        // Totals calculated on server
        public decimal Subtotal { get; set; }
        public decimal DeliveryCost { get; set; } = 15.00m;
        public decimal DiscountAmount { get; set; } = 0m;
        public decimal TotalAmount { get; set; }
        public decimal WalletBalance { get; set; } = 0m;

        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

