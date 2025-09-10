using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.Services;
using ShopForHome.ViewModels;
using System.Security.Claims;

namespace ShopForHome.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiPolicy")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("available")]
        public async Task<ActionResult<ApiResponse<List<CouponDto>>>> GetAvailableCoupons()
        {
            try
            {
                var userId = GetCurrentUserId();
                var coupons = await _couponService.GetUserAvailableCouponsAsync(userId);
                return Ok(ApiResponse<List<CouponDto>>.SuccessResponse(coupons, "Available coupons retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CouponDto>>.ErrorResponse("Error retrieving available coupons"));
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponse<CouponDto>>> ValidateCoupon([FromBody] ValidateCouponDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<CouponDto>.ErrorResponse("Invalid coupon code"));
                }

                var userId = GetCurrentUserId();
                var result = await _couponService.ValidateCouponAsync(model.CouponCode, userId);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<CouponDto>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<CouponDto>.SuccessResponse(result.Coupon, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CouponDto>.ErrorResponse("Error validating coupon"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }
    }
}
