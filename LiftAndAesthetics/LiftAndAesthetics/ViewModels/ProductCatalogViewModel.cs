using System;
using System.Collections.Generic;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.ViewModels
{
    public class ProductCatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalProducts { get; set; } = 0;
        public int PageSize { get; set; } = 12;

        public string? SearchQuery { get; set; }
        public string SelectedCategory { get; set; } = "All";
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; } = false;
        public string SortBy { get; set; } = "default"; // "default", "price_asc", "price_desc", "newest", "rating"

        public Dictionary<int, double> ProductRatings { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, int> ProductReviewCounts { get; set; } = new Dictionary<int, int>();
        public HashSet<int> UserWishlistProductIds { get; set; } = new HashSet<int>();

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

