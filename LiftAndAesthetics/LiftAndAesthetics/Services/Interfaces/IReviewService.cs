using System.Collections.Generic;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync(bool includeHidden = false);
        Task<IEnumerable<Review>> GetReviewsByCategoryAsync(string category, bool includeHidden = false);
        Task<Review?> GetReviewByIdAsync(int id);
        Task CreateReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);

        // B3: Reviews & Ratings on products and moderation
        Task<bool> CanUserReviewProductAsync(string userEmail, int productId);
        Task<double> GetAverageRatingForProductAsync(int productId);
        Task<int> GetReviewCountForProductAsync(int productId);
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId, bool includeHidden = false);
        Task ToggleHideReviewAsync(int reviewId);
        Task<Dictionary<int, double>> GetAllProductAverageRatingsAsync();
        Task<Dictionary<int, int>> GetAllProductReviewCountsAsync();
    }
}
