using System;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using LiftAndAesthetics.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftAndAesthetics.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IDiscountService _discountService;
        private readonly IRepositoryWrapper _repository;

        public CheckoutController(
            IOrderService orderService,
            ICartService cartService,
            IDiscountService discountService,
            IRepositoryWrapper repository)
        {
            _orderService = orderService;
            _cartService = cartService;
            _discountService = discountService;
            _repository = repository;
        }

        // A5: Checkout multi-step (Address, Delivery Method, Summary, Payment)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return RedirectToAction("Login", "Auth");

            var cartItems = (await _repository.Cart.GetByMemberIdAsync(member.MemberId)).ToList();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add products before checking out.";
                return RedirectToAction("Index", "Cart");
            }

            // Check stock availability (A3)
            foreach (var item in cartItems)
            {
                var prod = await _repository.Product.GetByIdAsync(item.ProductId);
                if (prod == null || prod.StockQuantity < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Product '{prod?.Name ?? "Item"}' has only {prod?.StockQuantity ?? 0} unit(s) in stock. Please adjust your cart.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            decimal subtotal = cartItems.Sum(c => c.Quantity * (c.Product?.Price ?? 0m));

            // Parse saved delivery address if available (A4)
            string savedAddress = member.Address ?? "";
            string savedCity = "Craiova";
            if (savedAddress.Contains(','))
            {
                var parts = savedAddress.Split(',');
                savedAddress = parts[0].Trim();
                if (parts.Length > 1) savedCity = parts[1].Trim();
            }

            var model = new CheckoutViewModel
            {
                CustomerName = member.Name,
                CustomerEmail = member.Email,
                DeliveryAddress = savedAddress,
                DeliveryCity = savedCity,
                DeliveryMethod = "Standard Delivery",
                DeliveryCost = 15.00m,
                PaymentMethod = "Credit Card (Sandbox)",
                Subtotal = subtotal,
                TotalAmount = subtotal + 15.00m,
                CartItems = cartItems
            };

            return View(model);
        }

        // B5: Discount validation AJAX endpoint
        [HttpPost]
        public async Task<IActionResult> ValidatePromoCode(string code, decimal subtotal)
        {
            var result = await _discountService.ValidateCodeAsync(code, subtotal);
            if (!result.IsValid || result.Discount == null)
            {
                return Json(new { success = false, message = result.Message });
            }

            decimal discountVal = _discountService.CalculateDiscount(result.Discount, subtotal);
            return Json(new
            {
                success = true,
                message = result.Message,
                discountAmount = discountVal,
                newTotal = Math.Max(0, subtotal - discountVal)
            });
        }

        // A5 & A6: Place Order step
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            string? userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                var cartItems = (await _repository.Cart.GetByMemberIdAsync(member.MemberId)).ToList();
                model.CartItems = cartItems;
                return View("Index", model);
            }

            var result = await _orderService.CreateOrderAsync(userEmail, model);
            if (!result.Success || result.Order == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            // Redirect to Sandbox Payment step (A6)
            return RedirectToAction(nameof(Payment), new { orderId = result.Order.OrderId });
        }

        // A6: Sandbox payment gateway simulation
        [HttpGet]
        public async Task<IActionResult> Payment(int orderId)
        {
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(orderId);
            if (order == null) return NotFound();

            string? userEmail = User.Identity?.Name;
            var member = await _repository.Member.GetByEmailAsync(userEmail!);
            if (member == null || (order.MemberId != member.MemberId && member.Role != "Admin"))
            {
                return Forbid();
            }

            if (order.Status == "Paid")
            {
                return RedirectToAction(nameof(Confirmation), new { id = order.OrderId });
            }

            ViewBag.WalletBalance = member.WalletBalance;
            return View(order);
        }

        // A6: Sandbox payment webhook / server confirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int orderId, string paymentMethod)
        {
            var result = await _orderService.ConfirmPaymentAsync(orderId, paymentMethod);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Confirmation), new { id = orderId });
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Payment), new { orderId = orderId });
        }

        // A6: Payment failure simulation (leaves order Unpaid)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateFailure(int orderId)
        {
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(orderId);
            if (order != null)
            {
                order.Status = "Unpaid";
                _repository.Order.Update(order);
                await _repository.SaveAsync();
            }

            TempData["ErrorMessage"] = "Sandbox payment was declined. The order remains Unpaid.";
            return RedirectToAction(nameof(Payment), new { orderId = orderId });
        }

        // A5: Confirmation page displaying the order number
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
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
    }
}

