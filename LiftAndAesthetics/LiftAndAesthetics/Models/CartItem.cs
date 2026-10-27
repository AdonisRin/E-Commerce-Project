using System.ComponentModel.DataAnnotations;

namespace LiftAndAesthetics.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int MemberId { get; set; }
        public Members? Member { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; } = 1;
    }
}