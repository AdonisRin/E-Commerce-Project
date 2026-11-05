using System.Collections.Generic;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.ViewModels;
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

        // Catalog, pagination, combinable filters & sorting (A1, A2, B1)
        Task<ProductCatalogViewModel> GetCatalogAsync(
            string? category,
            string? search,
            decimal? minPrice,
            decimal? maxPrice,
            bool inStockOnly,
            string? sortBy,
            int page = 1,
            int pageSize = 12,
            string? userEmail = null);

        // Product details & recommendations (B2, B3)
        Task<ProductDetailsViewModel?> GetProductDetailsAsync(int productId, string? userEmail = null);
        Task<IEnumerable<Product>> GetSimilarProductsAsync(int productId, int count = 4);
        Task<IEnumerable<Product>> GetFrequentlyBoughtTogetherAsync(int productId, int count = 3);
    }
}
