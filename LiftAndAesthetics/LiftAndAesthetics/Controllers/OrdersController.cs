using System;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftAndAesthetics.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IRepositoryWrapper _repository;

        public OrdersController(IOrderService orderService, IRepositoryWrapper repository)
        {
            _orderService = orderService;
            _repository = repository;
        }

        // A8: Customer Order History
        public async Task<IActionResult> Index()
        {
            string? userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            var orders = await _orderService.GetUserOrdersAsync(userEmail);
            return View(orders);
        }

        // Order Details
        public async Task<IActionResult> Details(int id)
        {
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(id);
            if (order == null) return NotFound();

            string? userEmail = User.Identity?.Name;
            var member = await _repository.Member.GetByEmailAsync(userEmail!);
            if (member == null || (order.MemberId != member.MemberId && member.Role != "Admin"))
            {
                return Forbid();
            }

            return View(order);
        }

        // A7: Order Cancellation (Restores stock)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            string? userEmail = User.Identity?.Name;
            var member = await _repository.Member.GetByEmailAsync(userEmail!);
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(id);

            if (order == null) return NotFound();
            if (member == null || (order.MemberId != member.MemberId && member.Role != "Admin"))
            {
                return Forbid();
            }

            if (order.Status == "Shipped" || order.Status == "Delivered")
            {
                TempData["ErrorMessage"] = $"Cannot cancel order #{order.OrderNumber} because it is already {order.Status}.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Order #{order.OrderNumber} has been cancelled and product stock has been restored.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
