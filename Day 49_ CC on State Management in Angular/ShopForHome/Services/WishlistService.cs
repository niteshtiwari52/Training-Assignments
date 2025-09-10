using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ShopForHomeDbContext _context;

        public WishlistService(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public async Task<List<WishlistItemDto>> GetUserWishlistAsync(int userId)
        {
            var wishlistItems = await _context.Wishlists
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return wishlistItems.Select(w => new WishlistItemDto
            {
                WishlistId = w.WishlistId,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductImage = w.Product.ImagePath,
                CategoryName = w.Product.Category.Name,
                Price = w.Product.Price,
                StockQuantity = w.Product.StockQuantity,
                AddedDate = w.CreatedAt
            }).ToList();
        }

        public async Task<(bool Success, string Message)> AddToWishlistAsync(int userId, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return (false, "Product not found");

            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingWishlistItem != null)
                return (false, "Product already in wishlist");

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return (true, "Product added to wishlist successfully");
        }

        public async Task<(bool Success, string Message)> RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
                return (false, "Product not found in wishlist");

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();
            return (true, "Product removed from wishlist successfully");
        }

        public async Task<(bool Success, string Message)> MoveToCartAsync(int userId, int productId, int quantity = 1)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
                return (false, "Product not found in wishlist");

            var cartService = new CartService(_context);
            var addResult = await cartService.AddToCartAsync(userId, productId, quantity);

            if (addResult.Success)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
                return (true, "Product moved to cart successfully");
            }

            return addResult;
        }
    }
}
