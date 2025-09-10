using System.ComponentModel.DataAnnotations;

namespace ShopForHome.ViewModels
{
    // Sales Report ViewModels
    public class SalesReportRequestViewModel
    {
        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Category Filter")]
        public int? CategoryId { get; set; }
    }

    public class SalesReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalGST { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<ProductSalesViewModel> TopProducts { get; set; } = new List<ProductSalesViewModel>();
        public List<CategorySalesViewModel> CategorySales { get; set; } = new List<CategorySalesViewModel>();
        public List<DailySalesViewModel> DailySales { get; set; } = new List<DailySalesViewModel>();
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new List<RecentOrderViewModel>();

        public int ReportDays => (ToDate - FromDate).Days + 1;
        public decimal DailyAverageRevenue => ReportDays > 0 ? TotalRevenue / ReportDays : 0;
    }

    public class ProductSalesViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AveragePrice => QuantitySold > 0 ? Revenue / QuantitySold : 0;
    }

    public class CategorySalesViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ProductCount { get; set; }
    }

    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public int ItemsSold { get; set; }
    }

    public class RecentOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; }
        public int ItemsCount { get; set; }
        public DateTime OrderDate { get; set; }

        public string StatusBadgeClass => Status switch
        {
            "Pending" => "bg-warning",
            "Confirmed" => "bg-info",
            "Shipped" => "bg-primary",
            "Delivered" => "bg-success",
            "Cancelled" => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
