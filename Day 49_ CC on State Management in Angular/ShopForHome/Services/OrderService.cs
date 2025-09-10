using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopForHomeDbContext _context;

        public OrderService(ShopForHomeDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderDto
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
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.ImagePath,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            }).ToList();
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null) return null;

            return new OrderDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                GST = order.GST,
                PayableAmount = order.PayableAmount,
                BalanceAmount = order.BalanceAmount,
                GrandTotal = order.GrandTotal,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message, int? OrderId)> CreateOrderFromCartAsync(int userId, List<int>? cartItemIds)
        {
            //var cartItems = await _context.Carts
            //    .Include(c => c.Product)
            //    .Where(c => c.UserId == userId)
            //    .ToListAsync();

            var cartQuery = _context.Carts
        .Include(c => c.Product)
        .Where(c => c.UserId == userId);

            // If specific cart items are selected, filter by those IDs
            if (cartItemIds != null && cartItemIds.Any())
            {
                cartQuery = cartQuery.Where(c => cartItemIds.Contains(c.CartId));
            }

            var cartItems = await cartQuery.ToListAsync();

            if (!cartItems.Any())
                return (false, "Cart is empty", null);

            // Check stock availability
            foreach (var item in cartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                    return (false, $"Insufficient stock for {item.Product.Name}", null);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create order
                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = cartItems.Sum(c => c.Price),
                    DiscountAmount = cartItems.Sum(c => c.DiscountAmount),
                    GST = cartItems.Sum(c => (c.Price * c.GST / 100)),
                    PayableAmount = cartItems.Sum(c => c.FinalPrice),
                    BalanceAmount = 0,
                    GrandTotal = cartItems.Sum(c => c.FinalPrice),
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items and update stock
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = cartItem.Product.Price
                    };

                    _context.OrderItems.Add(orderItem);

                    // Update product stock
                    cartItem.Product.StockQuantity -= cartItem.Quantity;

                    // Mark coupon as used if applicable
                    if (cartItem.CouponId.HasValue)
                    {
                        var userCoupon = await _context.UserCoupons
                            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == cartItem.CouponId.Value);
                        if (userCoupon != null)
                        {
                            userCoupon.IsUsed = true;
                        }

                        var coupon = await _context.Coupons.FindAsync(cartItem.CouponId.Value);
                        if (coupon != null)
                        {
                            coupon.TotalUsed++;
                        }
                    }
                }

                // Clear cart
                _context.Carts.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Order created successfully", order.OrderId);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Failed to create order", null);
            }
        }

        public async Task<(bool Success, string Message)> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                return (false, "Order not found");

            if (order.Status != "Pending")
                return (false, "Cannot cancel this order");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Restore stock
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product.StockQuantity += orderItem.Quantity;
                }

                order.Status = "Cancelled";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Order cancelled successfully");
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Failed to cancel order");
            }
        }
    }
}
