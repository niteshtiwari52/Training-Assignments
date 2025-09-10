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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrders()
        {
            try
            {
                var userId = GetCurrentUserId();
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<OrderDto>>.ErrorResponse("Error retrieving orders"));
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

        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetOrderByIdAsync(orderId, userId);

                if (order == null)
                {
                    return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
                }

                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("Error retrieving order"));
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<int>>> CreateOrder([FromBody] CreateOrderDto model = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartItemIds = model?.CartItemIds;
                var result = await _orderService.CreateOrderFromCartAsync(userId, cartItemIds);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<int>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<int>.SuccessResponse(result.OrderId.Value, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse("Error creating order"));
            }
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse<string>>> CancelOrder(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _orderService.CancelOrderAsync(orderId, userId);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse("Error cancel order"));
            }
        }
    }
}
