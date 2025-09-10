using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    // Order Service Interface
    public interface IOrderService
    {
        Task<List<OrderDto>> GetUserOrdersAsync(int userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId, int userId);
        Task<(bool Success, string Message, int? OrderId)> CreateOrderFromCartAsync(int userId, List<int> cartItemIds = null);
        Task<(bool Success, string Message)> CancelOrderAsync(int orderId, int userId);
    }
}
