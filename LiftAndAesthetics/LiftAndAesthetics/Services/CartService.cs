using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class CartService : ICartService
    {
        private readonly IRepositoryWrapper _repository;

        public CartService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CartItem>> GetUserCartAsync(string email)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            return user != null ? await _repository.Cart.GetByMemberIdAsync(user.MemberId) : new List<CartItem>();
        }

        public async Task<(bool Success, string Message)> AddItemToCartAsync(string email, int productId)
        {
            var user = await _repository.Member.GetByEmailAsync(email);
            var product = await _repository.Product.FindByCondition(p => p.ProductId == productId).FirstOrDefaultAsync();

            if (user == null || product == null) return (false, "Error: Member profile or product not found.");

            var existing = await _repository.Cart.GetExistingItemAsync(user.MemberId, productId);
            if (existing != null)
            {
                existing.Quantity++;
                _repository.Cart.Update(existing);
            }
            else
            {
                _repository.Cart.Create(new CartItem { MemberId = user.MemberId, ProductId = productId, Quantity = 1 });
            }

            _repository.Save();
            return (true, $"{product.Name} added to cart.");
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _repository.Cart.FindByCondition(c => c.CartItemId == cartItemId).FirstOrDefaultAsync();
            if (item != null)
            {
                _repository.Cart.Delete(item);
                _repository.Save();
            }
        }

        public async Task<(bool Success, string Message)> CheckoutAsync(string email)
        {
            var user = await _repository.Member.GetByEmailAsync(email);

            if (user == null) return (false, "Error: User profile not found!");

            var items = await _repository.Cart.GetByMemberIdAsync(user.MemberId);

            if (!items.Any()) return (false, "Your cart is completely empty.");

            decimal total = items.Sum(i => i.Quantity * (i.Product?.Price ?? 0));

            if (total <= 0) return (false, "Invalid order total.");

            if (user.WalletBalance >= total)
            {
                user.WalletBalance -= total;
                _repository.Member.Update(user);
                _repository.Cart.ClearCart(user.MemberId);

                _repository.Save();

                return (true, $"Payment success! You paid {total:0.00} lei. Remaining balance: {user.WalletBalance:0.00} lei.");
            }

            return (false, "Insufficient funds! Do you even lift?");
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _repository.Cart.FindByCondition(c => c.CartItemId == cartItemId).FirstOrDefaultAsync();

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                _repository.Cart.Update(item);
                _repository.Save();
            }
        }
    }
}