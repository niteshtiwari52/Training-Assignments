using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Services;
using ShopForHome.ViewModels;
using System.Security.Claims;

namespace ShopForHome.Controllers
{ 

    [Authorize(Policy = "AdminPolicy")]
    public class DashboardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ShopForHomeDbContext _context;

        public DashboardController(IUserService userService, IProductService productService, ShopForHomeDbContext context)
        {
            _userService = userService;
            _productService = productService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get dashboard statistics
                var users = await _userService.GetUsersAsync(1, 5);
                var products = await _productService.GetProductsAsync(1, 5);

                
                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var monthlyOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= firstDayOfMonth && o.CreatedAt <= lastDayOfMonth)
                    .ToListAsync();

                ViewBag.MonthlyTotalOrders = monthlyOrders.Count;
                ViewBag.MonthlyRevenue = monthlyOrders.Sum(o => o.TotalAmount);


                ViewBag.TotalUsers = users.TotalCount;
                ViewBag.TotalProducts = products.TotalCount;
                ViewBag.RecentUsers = users.Items;
                ViewBag.RecentProducts = products.Items;

                return View();
            }
            catch (Exception ex)
            {
                // Log the exception here
                ViewBag.ErrorMessage = "An error occurred while loading dashboard data.";
                return View();
            }
        }
    }

    

    

}
