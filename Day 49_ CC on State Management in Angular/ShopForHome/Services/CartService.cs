using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    // Cart Service Implementation
    public class CartService : ICartService
    {
        private readonly ShopForHomeDbContext _context;

        public CartService(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemDto>> GetUserCartAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Include(c => c.Coupon)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return cartItems.Select(c => new CartItemDto
            {
                CartId = c.CartId,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductImage = c.Product.ImagePath,
                CategoryName = c.Product.Category.Name,
                UnitPrice = c.Product.Price,
                Quantity = c.Quantity,
                Price = c.Price,
                GST = c.GST,
                DiscountAmount = c.DiscountAmount,
                FinalPrice = c.FinalPrice,
                CouponCode = c.Coupon?.Code,
                StockQuantity = c.Product.StockQuantity
            }).ToList();
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return (false, "Product not found");

            if (product.StockQuantity < quantity)
                return (false, "Insufficient stock");

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                var newQuantity = existingCartItem.Quantity + quantity;
                if (product.StockQuantity < newQuantity)
                    return (false, "Insufficient stock for requested quantity");

                existingCartItem.Quantity = newQuantity;
                existingCartItem.Price = product.Price * newQuantity;
                existingCartItem.FinalPrice = CalculateFinalPrice(existingCartItem.Price, existingCartItem.GST, existingCartItem.DiscountAmount);
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price * quantity,
                    GST = product.GST,
                    DiscountAmount = 0,
                    FinalPrice = CalculateFinalPrice(product.Price * quantity, product.GST, 0)
                };

                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return (true, "Product added to cart successfully");
        }

        public async Task<(bool Success, string Message)> UpdateCartItemAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                return await RemoveFromCartAsync(userId, productId);

            var cartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
                return (false, "Cart item not found");

            if (cartItem.Product.StockQuantity < quantity)
                return (false, "Insufficient stock");

            cartItem.Quantity = quantity;
            cartItem.Price = cartItem.Product.Price * quantity;
            cartItem.FinalPrice = CalculateFinalPrice(cartItem.Price, cartItem.GST, cartItem.DiscountAmount);

            await _context.SaveChangesAsync();
            return (true, "Cart item updated successfully");
        }

        public async Task<(bool Success, string Message)> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
                return (false, "Cart item not found");

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return (true, "Product removed from cart successfully");
        }

        public async Task<(bool Success, string Message)> ClearCartAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return (true, "Cart cleared successfully");
        }

        public async Task<CartSummaryDto> GetCartSummaryAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var summary = new CartSummaryDto
            {
                TotalItems = cartItems.Sum(c => c.Quantity),
                SubTotal = cartItems.Sum(c => c.Price),
                TotalDiscount = cartItems.Sum(c => c.DiscountAmount),
                TotalGST = cartItems.Sum(c => (c.Price * c.GST / 100)),
                GrandTotal = cartItems.Sum(c => c.FinalPrice),
                HasItems = cartItems.Any()
            };

            return summary;
        }

        public async Task<(bool Success, string Message)> ApplyCouponToCartAsync(int userId, string couponCode)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode &&
                                         c.ValidFrom <= DateTime.UtcNow &&
                                         c.ValidTo >= DateTime.UtcNow &&
                                         c.TotalUsed < c.MaxUses);

            if (coupon == null)
                return (false, "Invalid or expired coupon");

            // Check if user is eligible for this coupon
            if (!coupon.IsPublic)
            {
                var userCoupon = await _context.UserCoupons
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == coupon.CouponId && !uc.IsUsed);

                if (userCoupon == null)
                    return (false, "You are not eligible for this coupon");
            }

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return (false, "Cart is empty");

            foreach (var item in cartItems)
            {
                var discountAmount = (item.Price * coupon.DiscountPercent / 100);
                item.DiscountAmount = discountAmount;
                item.CouponId = coupon.CouponId;
                item.FinalPrice = CalculateFinalPrice(item.Price, item.GST, discountAmount);
            }

            await _context.SaveChangesAsync();
            return (true, $"Coupon applied successfully. {coupon.DiscountPercent}% discount applied");
        }

        private decimal CalculateFinalPrice(decimal price, decimal gstRate, decimal discountAmount)
        {
            var priceAfterDiscount = price - discountAmount;
            var gstAmount = priceAfterDiscount * gstRate / 100;
            return priceAfterDiscount + gstAmount;
        }
    }
}
