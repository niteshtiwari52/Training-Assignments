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
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<WishlistItemDto>>>> GetWishlist()
        {
            try
            {
                var userId = GetCurrentUserId();
                var wishlistItems = await _wishlistService.GetUserWishlistAsync(userId);
                return Ok(ApiResponse<List<WishlistItemDto>>.SuccessResponse(wishlistItems, "Wishlist retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<WishlistItemDto>>.ErrorResponse("Error retrieving wishlist"));
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<string>>> AddToWishlist([FromBody] AddToWishlistDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid model state"));
                }

                var userId = GetCurrentUserId();
                var result = await _wishlistService.AddToWishlistAsync(userId, model.ProductId);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error adding product to wishlist"));
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveFromWishlist(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error removing product from wishlist"));
            }
        }

        [HttpPost("move-to-cart")]
        public async Task<ActionResult<ApiResponse<string>>> MoveToCart([FromBody] MoveToCartDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid model state"));
                }

                var userId = GetCurrentUserId();
                var result = await _wishlistService.MoveToCartAsync(userId, model.ProductId, model.Quantity);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Error moving product to cart"));
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
