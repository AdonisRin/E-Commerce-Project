using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;
using LiftAndAesthetics.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            if (product.StockQuantity < 0) product.StockQuantity = 0;
            product.CreatedAt = DateTime.Now;

            _repository.Product.Create(product);
            await _repository.SaveAsync();
            return new ServiceResponse { Success = true, Message = "Product added successfully!" };
        }

        public async Task<ServiceResponse> UpdateProductAsync(Product product, IFormFile? imageFile)
        {
            if (product.Price <= 0) return new ServiceResponse { Success = false, Message = "Price must be positive!" };
            if (imageFile != null) product.ImageUrl = await UploadFile(imageFile);

            _repository.Product.Update(product);
            await _repository.SaveAsync();
            return new ServiceResponse { Success = true, Message = "Product updated successfully!" };
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _repository.Product.Delete(product);
                await _repository.SaveAsync();
            }
        }

        public async Task<ProductCatalogViewModel> GetCatalogAsync(
            string? category,
            string? search,
            decimal? minPrice,
            decimal? maxPrice,
            bool inStockOnly,
            string? sortBy,
            int page = 1,
            int pageSize = 12,
            string? userEmail = null)
        {
            if (pageSize <= 0 || pageSize > 24) pageSize = 12; // Acceptance criteria A1: at most 24 products per page

            var query = _repository.Product.FindAll().AsQueryable();

            // 1. Category Filter
            if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => p.Category == category);
            }

            // 2. Search Filter (A2: text search on product names)
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) || (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            // 3. Price Filter (B1) with range safety guarding against colossal numbers / SQL overflow
            const decimal MaxAllowedPrice = 999999m;
            decimal? safeMin = null;
            decimal? safeMax = null;

            if (minPrice.HasValue)
            {
                if (minPrice.Value > MaxAllowedPrice) safeMin = MaxAllowedPrice;
                else if (minPrice.Value > 0) safeMin = minPrice.Value;
            }

            if (maxPrice.HasValue)
            {
                if (maxPrice.Value > MaxAllowedPrice) safeMax = MaxAllowedPrice;
                else if (maxPrice.Value > 0) safeMax = maxPrice.Value;
            }

            if (safeMin.HasValue)
            {
                decimal minVal = safeMin.Value;
                query = query.Where(p => p.Price >= minVal);
            }
            if (safeMax.HasValue)
            {
                decimal maxVal = safeMax.Value;
                query = query.Where(p => p.Price <= maxVal);
            }

            // 4. Availability Filter (B1)
            if (inStockOnly)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

            // 5. Sorting (B1: price asc/desc, date added, default)
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.ProductId)
            };

            int totalProducts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Fetch Ratings & Review Counts (B3)
            var productIds = products.Select(p => p.ProductId).ToList();
            var reviews = await _repository.Review.FindAll()
                .Where(r => r.ProductId != null && productIds.Contains(r.ProductId.Value) && !r.IsHidden)
                .ToListAsync();

            var ratingsDict = new Dictionary<int, double>();
            var countsDict = new Dictionary<int, int>();

            foreach (var prod in products)
            {
                var prodReviews = reviews.Where(r => r.ProductId == prod.ProductId).ToList();
                if (prodReviews.Any())
                {
                    ratingsDict[prod.ProductId] = Math.Round(prodReviews.Average(r => r.Rating), 1);
                    countsDict[prod.ProductId] = prodReviews.Count;
                }
                else
                {
                    ratingsDict[prod.ProductId] = 5.0; // Default baseline rating
                    countsDict[prod.ProductId] = 0;
                }
            }

            // If sorted by rating, re-order the in-memory batch if requested
            if (sortBy == "rating")
            {
                products = products.OrderByDescending(p => ratingsDict[p.ProductId]).ToList();
            }

            // Fetch Wishlist Items for current user (B7)
            var wishlistIds = new HashSet<int>();
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var member = await _repository.Member.GetByEmailAsync(userEmail);
                if (member != null)
                {
                    var userWishlist = await _repository.Wishlist.GetByMemberIdAsync(member.MemberId);
                    wishlistIds = userWishlist.Select(w => w.ProductId).ToHashSet();
                }
            }

            return new ProductCatalogViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalProducts = totalProducts,
                PageSize = pageSize,
                SearchQuery = search,
                SelectedCategory = category ?? "All",
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStockOnly = inStockOnly,
                SortBy = sortBy ?? "default",
                ProductRatings = ratingsDict,
                ProductReviewCounts = countsDict,
                UserWishlistProductIds = wishlistIds
            };
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsAsync(int productId, string? userEmail = null)
        {
            var product = await GetProductByIdAsync(productId);
            if (product == null) return null;

            var reviews = await _repository.Review.FindAll()
                .Include(r => r.Member)
                .Where(r => r.ProductId == productId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            double avgRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 5.0;
            int reviewCount = reviews.Count;

            var similar = await GetSimilarProductsAsync(productId, 4);
            var freqBought = await GetFrequentlyBoughtTogetherAsync(productId, 3);

            bool inWishlist = false;
            bool canReview = false;

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var member = await _repository.Member.GetByEmailAsync(userEmail);
                if (member != null)
                {
                    var wishItem = await _repository.Wishlist.GetExistingItemAsync(member.MemberId, productId);
                    inWishlist = wishItem != null;

                    if (member.Role == "Admin")
                    {
                        canReview = true;
                    }
                    else
                    {
                        canReview = await _repository.Order.HasUserPurchasedProductAsync(member.MemberId, productId);
                    }
                }
            }

            return new ProductDetailsViewModel
            {
                Product = product,
                AverageRating = avgRating,
                ReviewCount = reviewCount,
                Reviews = reviews,
                SimilarProducts = similar,
                FrequentlyBoughtTogether = freqBought,
                IsInWishlist = inWishlist,
                CanUserReview = canReview
            };
        }

        public async Task<IEnumerable<Product>> GetSimilarProductsAsync(int productId, int count = 4)
        {
            var current = await GetProductByIdAsync(productId);
            if (current == null) return new List<Product>();

            return await _repository.Product.FindAll()
                .Where(p => p.ProductId != productId && p.Category == current.Category)
                .OrderByDescending(p => p.StockQuantity > 0)
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFrequentlyBoughtTogetherAsync(int productId, int count = 3)
        {
            // 1. Find orders containing this product
            var orderIds = await _repository.Order.FindAll()
                .Where(o => o.OrderItems.Any(oi => oi.ProductId == productId))
                .Select(o => o.OrderId)
                .Take(50)
                .ToListAsync();

            if (orderIds.Any())
            {
                var frequentlyPairedIds = await _repository.Order.FindAll()
                    .Where(o => orderIds.Contains(o.OrderId))
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductId != productId)
                    .GroupBy(oi => oi.ProductId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(count)
                    .ToListAsync();

                if (frequentlyPairedIds.Any())
                {
                    var items = await _repository.Product.FindAll()
                        .Where(p => frequentlyPairedIds.Contains(p.ProductId))
                        .ToListAsync();

                    if (items.Count >= count) return items;

                    // Fallback supplement if fewer items
                    var additional = await _repository.Product.FindAll()
                        .Where(p => p.ProductId != productId && !frequentlyPairedIds.Contains(p.ProductId))
                        .OrderByDescending(p => p.StockQuantity > 0)
                        .Take(count - items.Count)
                        .ToListAsync();

                    items.AddRange(additional);
                    return items;
                }
            }

            // Fallback: Return top products excluding current
            return await _repository.Product.FindAll()
                .Where(p => p.ProductId != productId)
                .OrderByDescending(p => p.StockQuantity > 0)
                .ThenByDescending(p => p.Price)
                .Take(count)
                .ToListAsync();
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(folder, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(fs); }
            return "/photos/" + fileName;
        }
    }
}
