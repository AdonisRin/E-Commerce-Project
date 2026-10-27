using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepositoryWrapper _repository;

        public ReviewService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _repository.Review.FindAll()
                .Include(r => r.Member)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByCategoryAsync(string category)
        {
            return await _repository.Review.FindByCondition(r => r.TargetCategory == category)
                .Include(r => r.Member)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
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
    }
}