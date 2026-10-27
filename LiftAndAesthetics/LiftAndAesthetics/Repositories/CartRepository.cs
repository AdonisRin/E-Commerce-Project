using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class CartRepository : RepositoryBase<CartItem>, ICartRepository
    {
        public CartRepository(LiftContext context) : base(context) { }

        public async Task<IEnumerable<CartItem>> GetByMemberIdAsync(int memberId)
        {
            return await LiftContext.CartItems.Include(c => c.Product)
                                          .Where(c => c.MemberId == memberId)
                                          .ToListAsync();
        }

        public async Task<CartItem?> GetExistingItemAsync(int memberId, int productId)
        {
            return await LiftContext.CartItems.FirstOrDefaultAsync(c => c.MemberId == memberId && c.ProductId == productId);
        }

        public void ClearCart(int memberId)
        {
            var items = LiftContext.CartItems.Where(c => c.MemberId == memberId);
            LiftContext.CartItems.RemoveRange(items);
        }
    }
}