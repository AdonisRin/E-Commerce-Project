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

            // A3: Quantities above available stock are rejected
            if (product.StockQuantity <= 0)
            {
                return (false, $"Product '{product.Name}' is out of stock.");
            }

            var existing = await _repository.Cart.GetExistingItemAsync(user.MemberId, productId);
            if (existing != null)
            {
                if (existing.Quantity + 1 > product.StockQuantity)
                {
                    return (false, $"Cannot add more. Only {product.StockQuantity} unit(s) available in stock.");
                }

                existing.Quantity++;
                _repository.Cart.Update(existing);
            }
            else
            {
                _repository.Cart.Create(new CartItem { MemberId = user.MemberId, ProductId = productId, Quantity = 1 });
            }

            await _repository.SaveAsync();
            return (true, $"{product.Name} added to cart.");
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _repository.Cart.FindByCondition(c => c.CartItemId == cartItemId).FirstOrDefaultAsync();
            if (item != null)
            {
                _repository.Cart.Delete(item);
                await _repository.SaveAsync();
            }
        }

        public async Task<(bool Success, string Message)> CheckoutAsync(string email)
        {
            var user = await _repository.Member.GetByEmailAsync(email);

            if (user == null) return (false, "Error: User profile not found!");

            var items = (await _repository.Cart.GetByMemberIdAsync(user.MemberId)).ToList();

            if (!items.Any()) return (false, "Your cart is completely empty.");

            // Verify stock
            foreach (var item in items)
            {
                if (item.Product != null && item.Quantity > item.Product.StockQuantity)
                {
                    return (false, $"Insufficient stock for '{item.Product.Name}'. Only {item.Product.StockQuantity} available.");
                }
            }

            decimal total = items.Sum(i => i.Quantity * (i.Product?.Price ?? 0));

            if (total <= 0) return (false, "Invalid order total.");

            if (user.WalletBalance >= total)
            {
                user.WalletBalance -= total;
                _repository.Member.Update(user);

                // Deduct stock
                foreach (var item in items)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity -= item.Quantity;
                        _repository.Product.Update(item.Product);
                    }
                }

                _repository.Cart.ClearCart(user.MemberId);
                await _repository.SaveAsync();

                return (true, $"Payment success! You paid {total:0.00} lei. Remaining balance: {user.WalletBalance:0.00} lei.");
            }

            return (false, "Insufficient funds! Please top up your wallet or use card checkout.");
        }

        public async Task<(bool Success, string Message)> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                await RemoveItemAsync(cartItemId);
                return (true, "Item removed from cart.");
            }

            var item = await _repository.Cart.FindByCondition(c => c.CartItemId == cartItemId)
                .Include(c => c.Product)
                .FirstOrDefaultAsync();

            if (item == null) return (false, "Cart item not found.");

            // A3: Quantities above available stock are rejected
            if (item.Product != null && quantity > item.Product.StockQuantity)
            {
                return (false, $"Cannot set quantity to {quantity}. Maximum available stock is {item.Product.StockQuantity}.");
            }

            item.Quantity = quantity;
            _repository.Cart.Update(item);
            await _repository.SaveAsync();

            return (true, "Quantity updated successfully.");
        }
    }
}
