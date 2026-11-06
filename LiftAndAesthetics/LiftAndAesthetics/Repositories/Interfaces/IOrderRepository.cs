using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId);
        Task<Order?> GetOrderByIdWithItemsAsync(int orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        Task<bool> HasUserPurchasedProductAsync(int memberId, int productId);
    }
}

