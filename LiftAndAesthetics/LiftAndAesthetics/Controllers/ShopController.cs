using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using LiftAndAesthetics.ViewModels;

namespace LiftAndAesthetics.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IWishlistService _wishlistService;

        public ShopController(
            IProductService productService,
            IReviewService reviewService,
            IOrderService orderService,
            IWishlistService wishlistService)
        {
            _productService = productService;
            _reviewService = reviewService;
            _orderService = orderService;
            _wishlistService = wishlistService;
        }

        // A1, A2, B1: Catalog with pagination, category pages, combinable filters & sorting
        [HttpGet]
        public async Task<IActionResult> Index(
            string? category = "All",
            string? search = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool inStock = false,
            string? sortBy = "default",
            int page = 1)
        {
            // Sanitize colossal or out-of-range prices to prevent SQL Server decimal overflow
            const decimal MaxAllowedPrice = 999999m;
            if (minPrice.HasValue)
            {
                if (minPrice.Value < 0) minPrice = 0;
                if (minPrice.Value > MaxAllowedPrice) minPrice = MaxAllowedPrice;
            }
            if (maxPrice.HasValue)
            {
                if (maxPrice.Value < 0) maxPrice = 0;
                if (maxPrice.Value > MaxAllowedPrice) maxPrice = MaxAllowedPrice;
            }

            string? userEmail = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
            var catalogVm = await _productService.GetCatalogAsync(
                category: category,
                search: search,
                minPrice: minPrice,
                maxPrice: maxPrice,
                inStockOnly: inStock,
                sortBy: sortBy,
                page: page,
                pageSize: 12, // Max 24 per page (A1 acceptance criteria)
                userEmail: userEmail);

            // A2 & B1: Live AJAX search support without page reload
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("ajax"))
            {
                return PartialView("_ProductGridPartial", catalogVm);
            }

            return View(catalogVm);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string query)
        {
            var allProducts = await _productService.GetAllProductsAsync();
            if (!string.IsNullOrEmpty(query))
            {
                allProducts = allProducts.Where(p =>
                    (p.Name != null && p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            return Json(allProducts);
        }

        // A1, B2, B3, B7: Product Details Page
        public async Task<IActionResult> Details(int id)
        {
            string? userEmail = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
            var detailsVm = await _productService.GetProductDetailsAsync(id, userEmail);
            if (detailsVm == null) return NotFound();

            return View(detailsVm);
        }

        // B3: Product Review Submission (Only customers who purchased product can review)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            string? userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            bool canReview = await _reviewService.CanUserReviewProductAsync(userEmail, productId);
            if (!canReview)
            {
                TempData["ErrorMessage"] = "Only verified customers who purchased this product can leave a review.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["ErrorMessage"] = "Please select a valid rating (1-5) and write a comment.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();

            // Find member
            var member = (await _reviewService.GetAllReviewsAsync(true))
                .Select(r => r.Member)
                .FirstOrDefault(m => m != null && m.Email == userEmail);

            // Create review
            var review = new Review
            {
                MemberId = member != null ? member.MemberId : 0,
                ProductId = productId,
                TargetCategory = "Shop",
                Rating = rating,
                Comment = comment.Trim(),
                CreatedAt = DateTime.Now
            };

            await _reviewService.CreateReviewAsync(review);
            TempData["SuccessMessage"] = "Thank you! Your verified review has been published.";
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        // B3: Admin Hide/Show Review Moderation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHideReview(int reviewId, int returnProductId)
        {
            await _reviewService.ToggleHideReviewAsync(reviewId);
            TempData["SuccessMessage"] = "Review visibility updated.";
            return RedirectToAction(nameof(Details), new { id = returnProductId });
        }

        // A9: Admin Orders Management with Status Filters & Transitions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Orders(string? status = "All")
        {
            var orders = await _orderService.GetAllOrdersAsync(status);
            ViewBag.CurrentStatus = status ?? "All";
            return View(orders);
        }

        // A9: Status Transitions: New -> Paid -> Shipped -> Delivered, or Cancelled
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus, string? currentFilter = "All")
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Orders), new { status = currentFilter });
        }

        // B4: Sales Dashboard (Daily sales chart, top 5 products, AOV)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard(string days = "30")
        {
            var dashboardVm = await _orderService.GetSalesDashboardDataAsync(days);
            return View(dashboardVm);
        }

        // CRUD Products (Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.AddProductAsync(product, imageFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Price", result.Message);
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _productService.UpdateProductAsync(product, imageFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Price", result.Message);
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
