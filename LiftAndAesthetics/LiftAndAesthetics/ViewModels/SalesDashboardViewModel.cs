using System.Collections.Generic;

namespace LiftAndAesthetics.ViewModels
{
    public class SalesDashboardViewModel
    {
        public string TimeInterval { get; set; } = "30"; // "7", "30", "all"

        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }

        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartRevenueData { get; set; } = new List<decimal>();
        public List<int> ChartOrdersData { get; set; } = new List<int>();

        public List<TopProductItem> TopProducts { get; set; } = new List<TopProductItem>();
    }

    public class TopProductItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal TotalSales { get; set; }
        public string? ImageUrl { get; set; }
    }
}

