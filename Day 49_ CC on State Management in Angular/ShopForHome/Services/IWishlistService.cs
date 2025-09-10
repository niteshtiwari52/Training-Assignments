using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    // Wishlist Service Interface
    public interface IWishlistService
    {
        Task<List<WishlistItemDto>> GetUserWishlistAsync(int userId);
        Task<(bool Success, string Message)> AddToWishlistAsync(int userId, int productId);
        Task<(bool Success, string Message)> RemoveFromWishlistAsync(int userId, int productId);
        Task<(bool Success, string Message)> MoveToCartAsync(int userId, int productId, int quantity = 1);
    }
}
