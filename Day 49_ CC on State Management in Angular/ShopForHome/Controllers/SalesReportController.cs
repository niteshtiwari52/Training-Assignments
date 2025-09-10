using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ShopForHome.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class SalesReportController : Controller
    {
        private readonly ShopForHomeDbContext _context;

        public SalesReportController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new SalesReportRequestViewModel
            {
                FromDate = DateTime.Now.AddDays(-30),
                ToDate = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(SalesReportRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (model.FromDate > model.ToDate)
            {
                ModelState.AddModelError("", "From date cannot be greater than to date");
                return View("Index", model);
            }

            var report = await GenerateSalesReportAsync(model.FromDate, model.ToDate, model.CategoryId);

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.SelectedCategoryId = model.CategoryId;
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View("Report", report);
        }

        private async Task<SalesReportViewModel> GenerateSalesReportAsync(DateTime fromDate, DateTime toDate, int? categoryId = null)
        {
            var ordersQuery = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Include(o => o.User)
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate);

            if (categoryId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderItems.Any(oi => oi.Product.CategoryId == categoryId.Value));
            }

            var orders = await ordersQuery.ToListAsync();

            var report = new SalesReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                TotalDiscount = orders.Sum(o => o.DiscountAmount),
                TotalGST = orders.Sum(o => o.GST),
                TotalItemsSold = orders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.GrandTotal) : 0
            };

            // Top Products
            report.TopProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.ImagePath })
                .Select(g => new ProductSalesViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    ProductImage = g.Key.ImagePath,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.PriceAtPurchase * oi.Quantity),
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(10)
                .ToList();

            // Category Sales
            report.CategorySales = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.Name })
                .Select(g => new CategorySalesViewModel
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.PriceAtPurchase * oi.Quantity),
                    ProductCount = g.Select(oi => oi.ProductId).Distinct().Count()
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToList();

            // Daily Sales
            report.DailySales = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesViewModel
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.GrandTotal),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Recent Orders
            report.RecentOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new RecentOrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.FullName,
                    GrandTotal = o.GrandTotal,
                    Status = o.Status,
                    ItemsCount = o.OrderItems.Sum(oi => oi.Quantity),
                    OrderDate = o.CreatedAt
                })
                .ToList();

            return report;
        }
    }

    
}
