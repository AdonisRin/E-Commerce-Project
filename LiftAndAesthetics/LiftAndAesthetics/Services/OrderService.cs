using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using LiftAndAesthetics.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LiftAndAesthetics.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IDiscountService _discountService;
        private readonly IEmailService _emailService;

        public OrderService(
            IRepositoryWrapper repository,
            IDiscountService discountService,
            IEmailService emailService)
        {
            _repository = repository;
            _discountService = discountService;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message, Order? Order)> CreateOrderAsync(string userEmail, CheckoutViewModel model)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return (false, "User profile not found.", null);

            var cartItems = (await _repository.Cart.GetByMemberIdAsync(member.MemberId)).ToList();
            if (!cartItems.Any())
            {
                return (false, "Your cart is empty.", null);
            }

            // Check stock availability for all items before creating order
            foreach (var item in cartItems)
            {
                var product = await _repository.Product.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return (false, "A product in your cart is no longer available.", null);
                }

                if (item.Quantity > product.StockQuantity)
                {
                    return (false, $"Insufficient stock for '{product.Name}'. Only {product.StockQuantity} unit(s) remaining.", null);
                }
            }

            decimal subtotal = cartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0m));

            // Validate discount code if provided
            decimal discountAmount = 0m;
            string? validCode = null;
            if (!string.IsNullOrWhiteSpace(model.DiscountCode))
            {
                var discResult = await _discountService.ValidateCodeAsync(model.DiscountCode, subtotal);
                if (discResult.IsValid && discResult.Discount != null)
                {
                    discountAmount = _discountService.CalculateDiscount(discResult.Discount, subtotal);
                    validCode = discResult.Discount.Code;
                }
            }

            decimal deliveryCost = model.DeliveryMethod == "Express Delivery" ? 25.00m : 15.00m;
            decimal totalAmount = Math.Max(0m, subtotal - discountAmount) + deliveryCost;

            // Generate unique Order Number
            string orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                MemberId = member.MemberId,
                OrderDate = DateTime.Now,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                DeliveryAddress = model.DeliveryAddress,
                DeliveryCity = model.DeliveryCity,
                DeliveryMethod = model.DeliveryMethod,
                DeliveryCost = deliveryCost,
                PaymentMethod = model.PaymentMethod,
                Subtotal = subtotal,
                DiscountAmount = discountAmount,
                DiscountCode = validCode,
                TotalAmount = totalAmount,
                Status = "New" // Starts as New until Sandbox confirmation
            };

            foreach (var item in cartItems)
            {
                decimal price = item.Product?.Price ?? 0m;
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = price,
                    TotalPrice = price * item.Quantity
                });
            }

            _repository.Order.Create(order);
            await _repository.SaveAsync();

            // Also save address back to member profile for convenience (A4)
            if (string.IsNullOrEmpty(member.Address) || member.Address != model.DeliveryAddress)
            {
                member.Address = $"{model.DeliveryAddress}, {model.DeliveryCity}";
                _repository.Member.Update(member);
                await _repository.SaveAsync();
            }

            return (true, "Order created successfully.", order);
        }

        public async Task<(bool Success, string Message)> ConfirmPaymentAsync(int orderId, string paymentMethod)
        {
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(orderId);
            if (order == null) return (false, "Order not found.");

            if (order.Status == "Paid" || order.Status == "Shipped" || order.Status == "Delivered")
            {
                return (true, "Order has already been confirmed and paid.");
            }

            if (order.Status == "Cancelled")
            {
                return (false, "Cannot process payment for a cancelled order.");
            }

            var member = await _repository.Member.GetByIdAsync(order.MemberId);
            if (member == null) return (false, "Customer not found.");

            // Concurrency check and stock deduction (A7)
            foreach (var item in order.OrderItems)
            {
                var product = await _repository.Product.GetByIdAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    order.Status = "Unpaid";
                    _repository.Order.Update(order);
                    await _repository.SaveAsync();
                    return (false, $"Payment could not be completed: Product '{item.Product?.Name}' is out of stock.");
                }
            }

            // If payment is via Wallet, check funds and deduct
            if (paymentMethod == "Wallet")
            {
                if (member.WalletBalance < order.TotalAmount)
                {
                    order.Status = "Unpaid";
                    _repository.Order.Update(order);
                    await _repository.SaveAsync();
                    return (false, $"Insufficient wallet balance ({member.WalletBalance:0.00} LEI). Total is {order.TotalAmount:0.00} LEI.");
                }

                member.WalletBalance -= order.TotalAmount;
                _repository.Member.Update(member);
            }

            // Deduct stock for all items
            foreach (var item in order.OrderItems)
            {
                var product = await _repository.Product.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    _repository.Product.Update(product);
                }
            }

            // Update status and confirmation timestamp
            order.Status = "Paid";
            order.PaymentConfirmedAt = DateTime.Now;
            order.PaymentMethod = paymentMethod;
            _repository.Order.Update(order);

            // Record promo code usage if applied
            if (!string.IsNullOrEmpty(order.DiscountCode))
            {
                await _discountService.RecordUsageAsync(order.DiscountCode);
            }

            // Clear the user's cart
            _repository.Cart.ClearCart(member.MemberId);

            await _repository.SaveAsync();

            // Send transactional email confirmation (B6)
            try
            {
                await _emailService.SendOrderConfirmationEmailAsync(order);
            }
            catch (Exception ex)
            {
                // Non-blocking: order remains paid even if SMTP host rejects email
            }

            return (true, $"Payment confirmed! Order #{order.OrderNumber} is now marked as Paid.");
        }

        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _repository.Order.GetOrderByIdWithItemsAsync(orderId);
            if (order == null) return (false, "Order not found.");

            string oldStatus = order.Status;

            // Stock restoration on cancellation (A7)
            if (newStatus == "Cancelled" && (oldStatus == "Paid" || oldStatus == "Shipped"))
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _repository.Product.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _repository.Product.Update(product);
                    }
                }
            }
            // If reactivating or manually setting to Paid from Unpaid/New, deduct stock
            else if (newStatus == "Paid" && (oldStatus == "New" || oldStatus == "Cancelled" || oldStatus == "Unpaid"))
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _repository.Product.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        if (product.StockQuantity < item.Quantity)
                        {
                            return (false, $"Cannot mark as Paid: Insufficient stock for '{product.Name}'.");
                        }
                        product.StockQuantity -= item.Quantity;
                        _repository.Product.Update(product);
                    }
                }
                order.PaymentConfirmedAt = DateTime.Now;
            }

            order.Status = newStatus;
            _repository.Order.Update(order);
            await _repository.SaveAsync();

            return (true, $"Order #{order.OrderNumber} status updated to '{newStatus}'.");
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userEmail)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return new List<Order>();

            return await _repository.Order.GetOrdersByMemberIdAsync(member.MemberId);
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            return await _repository.Order.GetOrderByNumberAsync(orderNumber);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _repository.Order.GetOrderByIdWithItemsAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? statusFilter = null)
        {
            var orders = await _repository.Order.GetAllOrdersWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
            {
                orders = orders.Where(o => o.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return orders;
        }

        public async Task<SalesDashboardViewModel> GetSalesDashboardDataAsync(string timeInterval = "30")
        {
            var allOrders = (await _repository.Order.GetAllOrdersWithDetailsAsync())
                .Where(o => o.Status != "Cancelled")
                .ToList();

            DateTime filterDate = timeInterval switch
            {
                "7" => DateTime.Now.AddDays(-7),
                "30" => DateTime.Now.AddDays(-30),
                _ => DateTime.MinValue
            };

            var filteredOrders = allOrders.Where(o => o.OrderDate >= filterDate).ToList();

            decimal totalRevenue = filteredOrders.Where(o => o.Status == "Paid" || o.Status == "Shipped" || o.Status == "Delivered").Sum(o => o.TotalAmount);
            int totalOrders = filteredOrders.Count;
            int totalItemsSold = filteredOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);
            decimal aov = totalOrders > 0 ? Math.Round(totalRevenue / totalOrders, 2) : 0m;

            // Generate daily sales points for chart
            var dailyGroups = filteredOrders
                .Where(o => o.Status == "Paid" || o.Status == "Shipped" || o.Status == "Delivered")
                .GroupBy(o => o.OrderDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            var chartLabels = new List<string>();
            var chartRevenue = new List<decimal>();
            var chartOrders = new List<int>();

            // If empty, supply placeholder dates for clean visual
            if (!dailyGroups.Any())
            {
                for (int i = 6; i >= 0; i--)
                {
                    chartLabels.Add(DateTime.Now.AddDays(-i).ToString("dd MMM"));
                    chartRevenue.Add(0m);
                    chartOrders.Add(0);
                }
            }
            else
            {
                foreach (var grp in dailyGroups)
                {
                    chartLabels.Add(grp.Key.ToString("dd MMM"));
                    chartRevenue.Add(grp.Sum(o => o.TotalAmount));
                    chartOrders.Add(grp.Count());
                }
            }

            // Top 5 products by quantity sold
            var topProducts = filteredOrders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductItem
                {
                    ProductId = g.Key,
                    Name = g.First().Product?.Name ?? $"Product #{g.Key}",
                    Category = g.First().Product?.Category ?? "Supplements",
                    ImageUrl = g.First().Product?.ImageUrl,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    TotalSales = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(tp => tp.UnitsSold)
                .Take(5)
                .ToList();

            return new SalesDashboardViewModel
            {
                TimeInterval = timeInterval,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalItemsSold = totalItemsSold,
                AverageOrderValue = aov,
                ChartLabels = chartLabels,
                ChartRevenueData = chartRevenue,
                ChartOrdersData = chartOrders,
                TopProducts = topProducts
            };
        }
    }
}

