using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public class CouponService : ICouponService
    {
        private readonly ShopForHomeDbContext _context;

        public CouponService(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public async Task<List<CouponDto>> GetUserAvailableCouponsAsync(int userId)
        {
            var publicCoupons = await _context.Coupons
                .Where(c => c.IsPublic &&
                           c.ValidFrom <= DateTime.UtcNow &&
                           c.ValidTo >= DateTime.UtcNow &&
                           c.TotalUsed < c.MaxUses)
                .ToListAsync();

            var userSpecificCoupons = await _context.UserCoupons
                .Include(uc => uc.Coupon)
                .Where(uc => uc.UserId == userId &&
                           !uc.IsUsed &&
                           uc.Coupon.ValidFrom <= DateTime.UtcNow &&
                           uc.Coupon.ValidTo >= DateTime.UtcNow &&
                           uc.Coupon.TotalUsed < uc.Coupon.MaxUses)
                .Select(uc => uc.Coupon)
                .ToListAsync();

            var allCoupons = publicCoupons.Concat(userSpecificCoupons).Distinct().ToList();

            return allCoupons.Select(c => new CouponDto
            {
                CouponId = c.CouponId,
                Code = c.Code,
                DiscountPercent = c.DiscountPercent,
                IsPublic = c.IsPublic,
                ValidFrom = c.ValidFrom,
                ValidTo = c.ValidTo,
                MaxUses = c.MaxUses,
                TotalUsed = c.TotalUsed,
                Description = $"Get {c.DiscountPercent}% off on your purchase"
            }).ToList();
        }

        public async Task<(bool Success, string Message, CouponDto Coupon)> ValidateCouponAsync(string couponCode, int userId)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode);

            if (coupon == null)
                return (false, "Invalid coupon code", null);

            if (coupon.ValidFrom > DateTime.UtcNow)
                return (false, "Coupon is not yet valid", null);

            if (coupon.ValidTo < DateTime.UtcNow)
                return (false, "Coupon has expired", null);

            if (coupon.TotalUsed >= coupon.MaxUses)
                return (false, "Coupon usage limit reached", null);

            if (!coupon.IsPublic)
            {
                var userCoupon = await _context.UserCoupons
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == coupon.CouponId);

                if (userCoupon == null)
                    return (false, "You are not eligible for this coupon", null);

                if (userCoupon.IsUsed)
                    return (false, "Coupon has already been used", null);
            }

            var couponDto = new CouponDto
            {
                CouponId = coupon.CouponId,
                Code = coupon.Code,
                DiscountPercent = coupon.DiscountPercent,
                IsPublic = coupon.IsPublic,
                ValidFrom = coupon.ValidFrom,
                ValidTo = coupon.ValidTo,
                MaxUses = coupon.MaxUses,
                TotalUsed = coupon.TotalUsed,
                Description = $"Get {coupon.DiscountPercent}% off on your purchase"
            };

            return (true, "Coupon is valid", couponDto);
        }

        public async Task<(bool Success, string Message)> UseCouponAsync(int userId, int couponId)
        {
            var userCoupon = await _context.UserCoupons
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId && !uc.IsUsed);

            if (userCoupon == null)
                return (false, "Coupon not found or already used");

            userCoupon.IsUsed = true;
            await _context.SaveChangesAsync();
            return (true, "Coupon marked as used");
        }
    }
}
