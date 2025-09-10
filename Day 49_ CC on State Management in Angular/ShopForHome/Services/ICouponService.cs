using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    // Coupon Service Interface
    public interface ICouponService
    {
        Task<List<CouponDto>> GetUserAvailableCouponsAsync(int userId);
        Task<(bool Success, string Message, CouponDto Coupon)> ValidateCouponAsync(string couponCode, int userId);
        Task<(bool Success, string Message)> UseCouponAsync(int userId, int couponId);
    }
}
