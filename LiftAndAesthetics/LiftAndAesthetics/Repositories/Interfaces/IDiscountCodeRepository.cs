using LiftAndAesthetics.Models;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IDiscountCodeRepository : IRepositoryBase<DiscountCode>
    {
        Task<DiscountCode?> GetByCodeAsync(string code);
    }
}

