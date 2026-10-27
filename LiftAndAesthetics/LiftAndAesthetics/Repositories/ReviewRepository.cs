using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;

namespace LiftAndAesthetics.Repositories
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {
        public ReviewRepository(LiftContext liftContext) : base(liftContext)
        {
        }
    }
}