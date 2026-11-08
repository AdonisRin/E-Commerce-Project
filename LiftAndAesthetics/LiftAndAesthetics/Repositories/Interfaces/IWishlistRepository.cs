using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IWishlistRepository : IRepositoryBase<WishlistItem>
    {
        Task<IEnumerable<WishlistItem>> GetByMemberIdAsync(int memberId);
        Task<WishlistItem?> GetExistingItemAsync(int memberId, int productId);
        Task RemoveItemAsync(int memberId, int productId);
    }
}

