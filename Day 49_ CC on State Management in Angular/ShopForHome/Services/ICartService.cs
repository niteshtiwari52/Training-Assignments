using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    // Cart Service Interface
    public interface ICartService
    {
        Task<List<CartItemDto>> GetUserCartAsync(int userId);
        Task<(bool Success, string Message)> AddToCartAsync(int userId, int productId, int quantity = 1);
        Task<(bool Success, string Message)> UpdateCartItemAsync(int userId, int productId, int quantity);
        Task<(bool Success, string Message)> RemoveFromCartAsync(int userId, int productId);
        Task<(bool Success, string Message)> ClearCartAsync(int userId);
        Task<CartSummaryDto> GetCartSummaryAsync(int userId);
        Task<(bool Success, string Message)> ApplyCouponToCartAsync(int userId, string couponCode);
    }
}
