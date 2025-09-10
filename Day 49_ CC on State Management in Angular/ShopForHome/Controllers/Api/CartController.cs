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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CartItemDto>>>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartItems = await _cartService.GetUserCartAsync(userId);
                return Ok(ApiResponse<List<CartItemDto>>.SuccessResponse(cartItems, "Cart retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<CartItemDto>>.ErrorResponse("Error retrieving cart"));
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<CartSummaryDto>>> GetCartSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartSummary = await _cartService.GetCartSummaryAsync(userId);
                return Ok(ApiResponse<CartSummaryDto>.SuccessResponse(cartSummary, "Cart summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartSummaryDto>.ErrorResponse("Error retrieving cart summary"));
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<string>>> AddToCart([FromBody] AddToCartDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid model state"));
                }

                var userId = GetCurrentUserId();
                var result = await _cartService.AddToCartAsync(userId, model.ProductId, model.Quantity);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error adding product to cart"));
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCartItem([FromBody] UpdateCartItemDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid model state"));
                }

                var userId = GetCurrentUserId();
                var result = await _cartService.UpdateCartItemAsync(userId, model.ProductId, model.Quantity);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error updating cart item"));
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.RemoveFromCartAsync(userId, productId);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error removing product from cart"));
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<string>>> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.ClearCartAsync(userId);

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error clearing cart"));
            }
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<ApiResponse<string>>> ApplyCoupon([FromBody] ApplyCouponDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid coupon code"));
                }

                var userId = GetCurrentUserId();
                var result = await _cartService.ApplyCouponToCartAsync(userId, model.CouponCode);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error applying coupon"));
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
