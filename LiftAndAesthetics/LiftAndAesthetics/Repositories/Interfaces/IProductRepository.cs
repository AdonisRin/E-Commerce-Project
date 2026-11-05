using LiftAndAesthetics.Models;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<Product?> GetByIdAsync(int id);
    }
}
