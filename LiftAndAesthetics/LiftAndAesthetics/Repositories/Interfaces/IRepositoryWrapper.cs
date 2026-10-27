using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IRepositoryWrapper
    {
        ITrainerRepository Trainer { get; }
        IProductRepository Product { get; }
        IFitnessClassRepository FitnessClass { get; }
        IMembershipRepository Membership { get; }
        IContactMessageRepository ContactMessage { get; }
        INutritionRepository Nutrition { get; }
        IMemberRepository Member { get; }
        ICartRepository Cart { get; }

        IChatRepository Chat { get; }
        IClassReservationRepository ClassReservation { get; }
        IReviewRepository Review { get; }

        void Save();
        Task SaveAsync();
    }
}