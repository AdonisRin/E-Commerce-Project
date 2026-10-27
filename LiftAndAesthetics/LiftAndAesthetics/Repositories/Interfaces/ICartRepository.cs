using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface ICartRepository : IRepositoryBase<CartItem>
    {
        Task<IEnumerable<CartItem>> GetByMemberIdAsync(int memberId);
        Task<CartItem?> GetExistingItemAsync(int memberId, int productId);
        void ClearCart(int memberId); 
    }
}