using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;
            var cartItems = await _cartService.GetUserCartAsync(email);

            ViewBag.CartTotal = cartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0));

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var email = User.Identity.Name;
            var result = await _cartService.AddItemToCartAsync(email, productId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity > 0)
            {
                await _cartService.UpdateQuantityAsync(cartItemId, quantity);
                TempData["SuccessMessage"] = "Quantity updated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveItemAsync(cartItemId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var email = User.Identity.Name;
            var result = await _cartService.CheckoutAsync(email);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}