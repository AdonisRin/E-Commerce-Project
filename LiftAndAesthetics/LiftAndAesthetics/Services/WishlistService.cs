using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;

namespace LiftAndAesthetics.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly ICartService _cartService;

        public WishlistService(IRepositoryWrapper repository, ICartService cartService)
        {
            _repository = repository;
            _cartService = cartService;
        }

        public async Task<IEnumerable<WishlistItem>> GetUserWishlistAsync(string userEmail)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return new List<WishlistItem>();

            return await _repository.Wishlist.GetByMemberIdAsync(member.MemberId);
        }

        public async Task<(bool Success, string Message)> AddToWishlistAsync(string userEmail, int productId)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return (false, "Please log in to add items to your wishlist.");

            var product = await _repository.Product.GetByIdAsync(productId);
            if (product == null) return (false, "Product not found.");

            var existing = await _repository.Wishlist.GetExistingItemAsync(member.MemberId, productId);
            if (existing != null)
            {
                return (false, $"{product.Name} is already in your wishlist.");
            }

            _repository.Wishlist.Create(new WishlistItem
            {
                MemberId = member.MemberId,
                ProductId = productId,
                AddedDate = DateTime.Now
            });
            await _repository.SaveAsync();

            return (true, $"{product.Name} saved to your wishlist!");
        }

        public async Task RemoveFromWishlistAsync(string userEmail, int productId)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return;

            await _repository.Wishlist.RemoveItemAsync(member.MemberId, productId);
            await _repository.SaveAsync();
        }

        public async Task<(bool Success, string Message)> MoveToCartAsync(string userEmail, int productId)
        {
            var cartResult = await _cartService.AddItemToCartAsync(userEmail, productId);
            if (cartResult.Success)
            {
                await RemoveFromWishlistAsync(userEmail, productId);
                return (true, $"{cartResult.Message} Moved from wishlist to cart.");
            }
            return cartResult;
        }

        public async Task<bool> IsInWishlistAsync(string userEmail, int productId)
        {
            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return false;

            var existing = await _repository.Wishlist.GetExistingItemAsync(member.MemberId, productId);
            return existing != null;
        }

        public async Task<HashSet<int>> GetUserWishlistProductIdsAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail)) return new HashSet<int>();

            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return new HashSet<int>();

            var items = await _repository.Wishlist.GetByMemberIdAsync(member.MemberId);
            return items.Select(w => w.ProductId).ToHashSet();
        }
    }
}

