using LiftAndAesthetics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<IEnumerable<Review>> GetReviewsByCategoryAsync(string category);
        Task<Review?> GetReviewByIdAsync(int id);
        Task CreateReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
    }
}