using System.Collections.Generic;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public double AverageRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public IEnumerable<Product> SimilarProducts { get; set; } = new List<Product>();
        public IEnumerable<Product> FrequentlyBoughtTogether { get; set; } = new List<Product>();
        public bool IsInWishlist { get; set; } = false;
        public bool CanUserReview { get; set; } = false;
    }
}

