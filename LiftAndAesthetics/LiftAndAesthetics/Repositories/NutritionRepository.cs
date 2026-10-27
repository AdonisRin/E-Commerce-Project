using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;

namespace LiftAndAesthetics.Repositories
{
    public class NutritionRepository : RepositoryBase<NutritionPlan>, INutritionRepository
    {
        public NutritionRepository(LiftContext context) : base(context)
        {
        }
    }
}