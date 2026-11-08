using System.Collections.Generic;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItem>> GetUserWishlistAsync(string userEmail);
        Task<(bool Success, string Message)> AddToWishlistAsync(string userEmail, int productId);
        Task RemoveFromWishlistAsync(string userEmail, int productId);
        Task<(bool Success, string Message)> MoveToCartAsync(string userEmail, int productId);
        Task<bool> IsInWishlistAsync(string userEmail, int productId);
        Task<HashSet<int>> GetUserWishlistProductIdsAsync(string userEmail);
    }
}

