using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(LiftContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId)
        {
            return await LiftContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.MemberId == memberId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdWithItemsAsync(int orderId)
        {
            return await LiftContext.Orders
                .Include(o => o.Member)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            return await LiftContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await LiftContext.Orders
                .Include(o => o.Member)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int memberId, int productId)
        {
            return await LiftContext.Orders
                .Where(o => o.MemberId == memberId && (o.Status == "Paid" || o.Status == "Shipped" || o.Status == "Delivered"))
                .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
        }
    }
}

