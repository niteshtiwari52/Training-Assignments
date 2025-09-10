using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.ViewModels;
using System.Security.Claims;

namespace ShopForHome.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiPolicy")]
    public class UserController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;
        private readonly ILogger<UserController> _logger;



        public UserController(ShopForHomeDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserProfile()
        {
            try
            {
                // FIXED: Better logging and debugging
                _logger.LogInformation("Getting user profile. User authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
                _logger.LogInformation("User claims: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));


                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<UserDetailDto>
                    {
                        Success = false,
                        Message = "Invalid user token",
                        Data = null
                    });
                }

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<UserDetailDto>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null
                    });
                }

                var userDto = new UserDetailDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Roles = user.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => !string.IsNullOrEmpty(r)).ToList() ?? new List<string>(),
                    CreatedAt = user.CreatedAt
                };

                return Ok(new ApiResponse<UserDetailDto>
                {
                    Success = true,
                    Message = "User profile retrieved successfully",
                    Data = userDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user profile",
                    Data = null
                });
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetUserOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<List<OrderDto>>
                    {
                        Success = false,
                        Message = "Invalid user token",
                        Data = null
                    });
                }

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var orderDtos = orders.Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    GST = o.GST,
                    PayableAmount = o.PayableAmount,
                    BalanceAmount = o.BalanceAmount,
                    GrandTotal = o.GrandTotal,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "",
                        Quantity = oi.Quantity,
                        PriceAtPurchase = oi.PriceAtPurchase
                    }).ToList() ?? new List<OrderItemDto>()
                }).ToList();

                return Ok(new ApiResponse<List<OrderDto>>
                {
                    Success = true,
                    Message = "User orders retrieved successfully",
                    Data = orderDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<OrderDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user orders",
                    Data = null
                });
            }
        }
    }
}
