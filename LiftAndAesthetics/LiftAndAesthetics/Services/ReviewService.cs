using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftAndAesthetics.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepositoryWrapper _repository;

        public ReviewService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync(bool includeHidden = false)
        {
            var query = _repository.Review.FindAll().Include(r => r.Member).AsQueryable();
            if (!includeHidden)
            {
                query = query.Where(r => !r.IsHidden);
            }
            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByCategoryAsync(string category, bool includeHidden = false)
        {
            var query = _repository.Review.FindByCondition(r => r.TargetCategory == category)
                .Include(r => r.Member)
                .AsQueryable();

            if (!includeHidden)
            {
                query = query.Where(r => !r.IsHidden);
            }

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _repository.Review.FindByCondition(r => r.ReviewId == id)
                .Include(r => r.Member)
                .FirstOrDefaultAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            _repository.Review.Create(review);
            await _repository.SaveAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            review.Member = null;
            _repository.Review.Update(review);
            await _repository.SaveAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _repository.Review.FindByCondition(r => r.ReviewId == id).FirstOrDefaultAsync();
            if (review != null)
            {
                _repository.Review.Delete(review);
                await _repository.SaveAsync();
            }
        }

        public async Task<bool> CanUserReviewProductAsync(string userEmail, int productId)
        {
            if (string.IsNullOrWhiteSpace(userEmail)) return false;

            var member = await _repository.Member.GetByEmailAsync(userEmail);
            if (member == null) return false;

            // Admin can always review/test
            if (member.Role == "Admin") return true;

            // Only customers who purchased a product may review it (B3)
            return await _repository.Order.HasUserPurchasedProductAsync(member.MemberId, productId);
        }

        public async Task<double> GetAverageRatingForProductAsync(int productId)
        {
            var ratings = await _repository.Review.FindAll()
                .Where(r => r.ProductId == productId && !r.IsHidden)
                .Select(r => r.Rating)
                .ToListAsync();

            if (!ratings.Any()) return 5.0; // Default high aesthetic rating for showcase if unreviewed
            return Math.Round(ratings.Average(), 1);
        }

        public async Task<int> GetReviewCountForProductAsync(int productId)
        {
            return await _repository.Review.FindAll()
                .CountAsync(r => r.ProductId == productId && !r.IsHidden);
        }

        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId, bool includeHidden = false)
        {
            var query = _repository.Review.FindAll()
                .Include(r => r.Member)
                .Where(r => r.ProductId == productId);

            if (!includeHidden)
            {
                query = query.Where(r => !r.IsHidden);
            }

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task ToggleHideReviewAsync(int reviewId)
        {
            var review = await _repository.Review.FindByCondition(r => r.ReviewId == reviewId).FirstOrDefaultAsync();
            if (review != null)
            {
                review.IsHidden = !review.IsHidden;
                _repository.Review.Update(review);
                await _repository.SaveAsync();
            }
        }

        public async Task<Dictionary<int, double>> GetAllProductAverageRatingsAsync()
        {
            var reviews = await _repository.Review.FindAll()
                .Where(r => r.ProductId != null && !r.IsHidden)
                .ToListAsync();

            var result = new Dictionary<int, double>();
            var grouped = reviews.GroupBy(r => r.ProductId!.Value);
            foreach (var g in grouped)
            {
                result[g.Key] = Math.Round(g.Average(r => r.Rating), 1);
            }
            return result;
        }

        public async Task<Dictionary<int, int>> GetAllProductReviewCountsAsync()
        {
            var reviews = await _repository.Review.FindAll()
                .Where(r => r.ProductId != null && !r.IsHidden)
                .ToListAsync();

            var result = new Dictionary<int, int>();
            var grouped = reviews.GroupBy(r => r.ProductId!.Value);
            foreach (var g in grouped)
            {
                result[g.Key] = g.Count();
            }
            return result;
        }
    }
}
