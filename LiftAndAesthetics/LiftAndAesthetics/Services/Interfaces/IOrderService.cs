using System.Collections.Generic;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.ViewModels;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(bool Success, string Message, Order? Order)> CreateOrderAsync(string userEmail, CheckoutViewModel model);
        Task<(bool Success, string Message)> ConfirmPaymentAsync(int orderId, string paymentMethod);
        Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userEmail);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync(string? statusFilter = null);
        Task<SalesDashboardViewModel> GetSalesDashboardDataAsync(string timeInterval = "30");
    }
}

