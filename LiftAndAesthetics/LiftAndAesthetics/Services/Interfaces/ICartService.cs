using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetUserCartAsync(string email);
        Task<(bool Success, string Message)> AddItemToCartAsync(string email, int productId);
        Task RemoveItemAsync(int cartItemId);
        Task<(bool Success, string Message)> CheckoutAsync(string email);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
    }
}