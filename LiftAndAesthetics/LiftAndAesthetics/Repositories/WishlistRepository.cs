using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class WishlistRepository : RepositoryBase<WishlistItem>, IWishlistRepository
    {
        public WishlistRepository(LiftContext context) : base(context) { }

        public async Task<IEnumerable<WishlistItem>> GetByMemberIdAsync(int memberId)
        {
            return await LiftContext.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.MemberId == memberId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();
        }

        public async Task<WishlistItem?> GetExistingItemAsync(int memberId, int productId)
        {
            return await LiftContext.WishlistItems
                .FirstOrDefaultAsync(w => w.MemberId == memberId && w.ProductId == productId);
        }

        public async Task RemoveItemAsync(int memberId, int productId)
        {
            var item = await GetExistingItemAsync(memberId, productId);
            if (item != null)
            {
                LiftContext.WishlistItems.Remove(item);
            }
        }
    }
}

