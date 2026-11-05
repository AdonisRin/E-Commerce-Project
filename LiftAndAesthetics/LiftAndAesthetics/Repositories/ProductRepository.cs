using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(LiftContext context) : base(context) { }

        public async Task<Product?> GetByIdAsync(int id) =>
            await LiftContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    }
}
