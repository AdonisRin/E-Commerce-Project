using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;

namespace LiftAndAesthetics.Repositories
{
    public class TrainerRepository : RepositoryBase<Trainer>, ITrainerRepository
    {
        public TrainerRepository(LiftContext context) : base(context) { }
    }
}