using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;

namespace LiftAndAesthetics.Repositories
{
    public class FitnessClassRepository : RepositoryBase<FitnessClass>, IFitnessClassRepository
    {
        public FitnessClassRepository(LiftContext context) : base(context) { }
    }
}