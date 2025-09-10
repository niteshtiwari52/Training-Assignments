using System.ComponentModel.DataAnnotations;

namespace ShopForHome.ViewModels
{
    // API DTOs
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal GST { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, T data = default(T))
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // Cart DTOs
    public class CartItemDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string CouponCode { get; set; }
        public int StockQuantity { get; set; }
        public decimal GSTAmount => (Price - DiscountAmount) * GST / 100;
        public decimal TotalPrice => FinalPrice;
    }

    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalGST { get; set; }
        public decimal GrandTotal { get; set; }
        public bool HasItems { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class ApplyCouponDto
    {
        [Required]
        [StringLength(50)]
        public string CouponCode { get; set; }
    }

    // Wishlist DTOs
    public class WishlistItemDto
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsInStock => StockQuantity > 0;
    }

    public class AddToWishlistDto
    {
        [Required]
        public int ProductId { get; set; }
    }

    public class MoveToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }

    // Enhanced Order DTOs
    public class OrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GST { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        // Additional calculated properties
        public int TotalItems => OrderItems?.Sum(oi => oi.Quantity) ?? 0;
        public bool CanCancel => Status == "Pending";
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

    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal TotalPrice => PriceAtPurchase * Quantity;
    }

    public class CreateOrderDto
    {
        //public string ShippingAddress { get; set; }
        //public string PaymentMethod { get; set; }
        //public string Notes { get; set; }
        public List<int> CartItemIds { get; set; }
    }

    // Coupon DTOs
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsPublic { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxUses { get; set; }
        public int TotalUsed { get; set; }
        public string Description { get; set; }

        // Calculated properties
        public bool IsActive => ValidFrom <= DateTime.UtcNow && ValidTo >= DateTime.UtcNow && TotalUsed < MaxUses;
        public int RemainingUses => Math.Max(0, MaxUses - TotalUsed);
        public string ValidityText => IsActive ? $"Valid till {ValidTo:MMM dd, yyyy}" : "Expired";
        public string CouponType => IsPublic ? "Public" : "Personal";
    }

    public class ValidateCouponDto
    {
        [Required]
        [StringLength(50)]
        public string CouponCode { get; set; }
    }

    // Search and Filter DTOs
    public class ProductSearchDto
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStockOnly { get; set; }
        public string SortBy { get; set; } = "name"; // name, price, date
        public string SortOrder { get; set; } = "asc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ProductSearchResultDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public List<CategorySummaryDto> Categories { get; set; } = new List<CategorySummaryDto>();
        public PriceRangeDto PriceRange { get; set; }
    }

    public class CategorySummaryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }
    }

    public class PriceRangeDto
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }

    // Sales Report DTOs (for Admin APIs)
    public class SalesReportRequestDto
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int? CategoryId { get; set; }
        public string ReportType { get; set; } = "summary"; // summary, detailed, product-wise
    }

    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalGST { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<ProductSalesDto> TopProducts { get; set; } = new List<ProductSalesDto>();
        public List<CategorySalesDto> CategorySales { get; set; } = new List<CategorySalesDto>();
        public List<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
    }

    public class ProductSalesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CategorySalesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public int ItemsSold { get; set; }
    }

    // Bulk Product Upload DTOs
    public class BulkProductUploadDto
    {
        [Required]
        public IFormFile CsvFile { get; set; }

        public bool SkipErrors { get; set; } = true;
        public bool UpdateExisting { get; set; } = false;
    }

    public class BulkUploadResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessfulRows { get; set; }
        public int ErrorRows { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasErrors => ErrorRows > 0;
        public string Summary => $"Successfully processed {SuccessfulRows} out of {TotalRows} products";
    }

    public class BulkProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal GST { get; set; }
        public string ImageUrl { get; set; }
    }

    // Admin Coupon Management DTOs
    public class CreateCouponDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal DiscountPercent { get; set; }

        public bool IsPublic { get; set; } = true;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxUses { get; set; } = 1;

        public List<int> AssignToUserIds { get; set; } = new List<int>();
    }

    public class UpdateCouponDto
    {
        public int CouponId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal DiscountPercent { get; set; }

        public bool IsPublic { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxUses { get; set; }
    }

    public class AssignCouponDto
    {
        [Required]
        public int CouponId { get; set; }

        [Required]
        public List<int> UserIds { get; set; }
    }

    public class CouponViewModel
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsPublic { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxUses { get; set; }
        public int TotalUsed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int AssignedUsersCount { get; set; }
        public int RemainingUses => Math.Max(0, MaxUses - TotalUsed);
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-secondary";
        public string TypeBadgeClass => IsPublic ? "bg-primary" : "bg-info";
    }

    public class CreateCouponViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Coupon Code")]
        public string Code { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercent { get; set; }

        [Display(Name = "Public Coupon")]
        public bool IsPublic { get; set; } = true;

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.DateTime)]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Display(Name = "Valid To")]
        [DataType(DataType.DateTime)]
        public DateTime ValidTo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Maximum Uses")]
        public int MaxUses { get; set; } = 1;
    }

    public class UpdateCouponViewModel
    {
        public int CouponId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Coupon Code")]
        public string Code { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercent { get; set; }

        [Display(Name = "Public Coupon")]
        public bool IsPublic { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.DateTime)]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Display(Name = "Valid To")]
        [DataType(DataType.DateTime)]
        public DateTime ValidTo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Maximum Uses")]
        public int MaxUses { get; set; }
    }

    public class CouponDetailsViewModel
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsPublic { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxUses { get; set; }
        public int TotalUsed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<AssignedUserViewModel> AssignedUsers { get; set; } = new List<AssignedUserViewModel>();
        public int RemainingUses => Math.Max(0, MaxUses - TotalUsed);
        public decimal UsagePercentage => MaxUses > 0 ? (decimal)TotalUsed / MaxUses * 100 : 0;
    }

    public class AssignedUserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AssignedDate { get; set; }
    }

    public class AssignUsersViewModel
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public List<UserSelectViewModel> Users { get; set; } = new List<UserSelectViewModel>();
        public List<int> SelectedUserIds { get; set; } = new List<int>();
    }

    public class UserSelectViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }

    // Response wrapper for consistent API responses
    //public class ApiResponse<T>
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //    public T Data { get; set; }
    //    public object Metadata { get; set; }

    //    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
    //    {
    //        return new ApiResponse<T>
    //        {
    //            Success = true,
    //            Message = message,
    //            Data = data
    //        };
    //    }

    //    public static ApiResponse<T> ErrorResponse(string message, T data = default(T))
    //    {
    //        return new ApiResponse<T>
    //        {
    //            Success = false,
    //            Message = message,
    //            Data = data
    //        };
    //    }
    //}

    // Pagination helper
    //public class PagedResult<T>
    //{
    //    public List<T> Items { get; set; } = new List<T>();
    //    public int TotalCount { get; set; }
    //    public int PageNumber { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    //    public bool HasPreviousPage => PageNumber > 1;
    //    public bool HasNextPage => PageNumber < TotalPages;
    //}
}
