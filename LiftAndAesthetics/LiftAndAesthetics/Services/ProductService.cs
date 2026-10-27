using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftAndAesthetics.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(IRepositoryWrapper repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
            await _repository.Product.FindAll().ToListAsync();

        public async Task<Product?> GetProductByIdAsync(int id) =>
            await _repository.Product.FindByCondition(p => p.ProductId == id).FirstOrDefaultAsync();

        public async Task<ServiceResponse> AddProductAsync(Product product, IFormFile? imageFile)
        {
            if (product.Price <= 0) return new ServiceResponse { Success = false, Message = "Price must be positive!" };
            if (imageFile != null) product.ImageUrl = await UploadFile(imageFile);

            _repository.Product.Create(product);
            _repository.Save();
            return new ServiceResponse { Success = true, Message = "Product added!" };
        }

        public async Task<ServiceResponse> UpdateProductAsync(Product product, IFormFile? imageFile)
        {
            if (product.Price <= 0) return new ServiceResponse { Success = false, Message = "Price must be positive!" };
            if (imageFile != null) product.ImageUrl = await UploadFile(imageFile);

            _repository.Product.Update(product);
            _repository.Save();
            return new ServiceResponse { Success = true, Message = "Product updated!" };
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _repository.Product.Delete(product);
                _repository.Save();
            }
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(folder, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(fs); }
            return "/photos/" + fileName;
        }
    }
}