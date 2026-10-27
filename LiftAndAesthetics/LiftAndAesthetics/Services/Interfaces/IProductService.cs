using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Http;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<ServiceResponse> AddProductAsync(Product product, IFormFile? imageFile);
        Task<ServiceResponse> UpdateProductAsync(Product product, IFormFile? imageFile);
        Task DeleteProductAsync(int id);
    }
}