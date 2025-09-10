using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;
using System.Security.Claims;

namespace ShopForHome.Controllers
{

    [Authorize(Policy = "AdminPolicy")]
    public class CouponManagementController : Controller
        {
            private readonly ShopForHomeDbContext _context;

            public CouponManagementController(ShopForHomeDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
            {
                var query = _context.Coupons
                    .Include(c => c.Creator)
                    .AsQueryable();

                var totalCount = await query.CountAsync();
                var coupons = await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CouponViewModel
                    {
                        CouponId = c.CouponId,
                        Code = c.Code,
                        DiscountPercent = c.DiscountPercent,
                        IsPublic = c.IsPublic,
                        ValidFrom = c.ValidFrom,
                        ValidTo = c.ValidTo,
                        MaxUses = c.MaxUses,
                        TotalUsed = c.TotalUsed,
                        CreatedBy = c.Creator.FullName,
                        CreatedAt = c.CreatedAt,
                        IsActive = c.ValidFrom <= DateTime.UtcNow && c.ValidTo >= DateTime.UtcNow && c.TotalUsed < c.MaxUses,
                        AssignedUsersCount = c.UserCoupons.Count(uc => !uc.IsUsed)
                    })
                    .ToListAsync();

                var pagedResult = new PagedResult<CouponViewModel>
                {
                    Items = coupons,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return View(pagedResult);
            }

            [HttpGet]
            public IActionResult Create()
            {
                var model = new CreateCouponViewModel
                {
                    ValidFrom = DateTime.Now,
                    ValidTo = DateTime.Now.AddDays(30),
                    MaxUses = 1,
                    IsPublic = true
                };

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Create(CreateCouponViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Check if coupon code already exists
                var existingCoupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == model.Code.ToLower());

                if (existingCoupon != null)
                {
                    ModelState.AddModelError("Code", "Coupon code already exists");
                    return View(model);
                }

                var adminUserId = GetCurrentUserId();

                var coupon = new Coupon
                {
                    Code = model.Code.ToUpper(),
                    DiscountPercent = model.DiscountPercent,
                    IsPublic = model.IsPublic,
                    ValidFrom = model.ValidFrom,
                    ValidTo = model.ValidTo,
                    MaxUses = model.MaxUses,
                    TotalUsed = 0,
                    CreatedBy = adminUserId
                };

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Coupon created successfully";
                return RedirectToAction("Index");
            }

            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                    return NotFound();

                var model = new UpdateCouponViewModel
                {
                    CouponId = coupon.CouponId,
                    Code = coupon.Code,
                    DiscountPercent = coupon.DiscountPercent,
                    IsPublic = coupon.IsPublic,
                    ValidFrom = coupon.ValidFrom,
                    ValidTo = coupon.ValidTo,
                    MaxUses = coupon.MaxUses
                };

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(UpdateCouponViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var coupon = await _context.Coupons.FindAsync(model.CouponId);
                if (coupon == null)
                    return NotFound();

                // Check if new code already exists (excluding current coupon)
                var existingCoupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == model.Code.ToLower() && c.CouponId != model.CouponId);

                if (existingCoupon != null)
                {
                    ModelState.AddModelError("Code", "Coupon code already exists");
                    return View(model);
                }

                coupon.Code = model.Code.ToUpper();
                coupon.DiscountPercent = model.DiscountPercent;
                coupon.IsPublic = model.IsPublic;
                coupon.ValidFrom = model.ValidFrom;
                coupon.ValidTo = model.ValidTo;
                coupon.MaxUses = model.MaxUses;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Coupon updated successfully";
                return RedirectToAction("Index");
            }

            public async Task<IActionResult> Details(int id)
            {
                var coupon = await _context.Coupons
                    .Include(c => c.Creator)
                    .Include(c => c.UserCoupons)
                    .ThenInclude(uc => uc.User)
                    .FirstOrDefaultAsync(c => c.CouponId == id);

                if (coupon == null)
                    return NotFound();

                var model = new CouponDetailsViewModel
                {
                    CouponId = coupon.CouponId,
                    Code = coupon.Code,
                    DiscountPercent = coupon.DiscountPercent,
                    IsPublic = coupon.IsPublic,
                    ValidFrom = coupon.ValidFrom,
                    ValidTo = coupon.ValidTo,
                    MaxUses = coupon.MaxUses,
                    TotalUsed = coupon.TotalUsed,
                    CreatedBy = coupon.Creator.FullName,
                    CreatedAt = coupon.CreatedAt,
                    IsActive = coupon.ValidFrom <= DateTime.UtcNow && coupon.ValidTo >= DateTime.UtcNow && coupon.TotalUsed < coupon.MaxUses,
                    AssignedUsers = coupon.UserCoupons.Select(uc => new AssignedUserViewModel
                    {
                        UserId = uc.UserId,
                        UserName = uc.User.FullName,
                        Email = uc.User.Email,
                        IsUsed = uc.IsUsed,
                        AssignedDate = uc.CreatedAt
                    }).ToList()
                };

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                {
                    TempData["ErrorMessage"] = "Coupon not found";
                    return RedirectToAction("Index");
                }

                // Check if coupon has been used
                if (coupon.TotalUsed > 0)
                {
                    TempData["ErrorMessage"] = "Cannot delete coupon that has been used";
                    return RedirectToAction("Index");
                }

                // Remove user assignments first
                var userCoupons = await _context.UserCoupons
                    .Where(uc => uc.CouponId == id)
                    .ToListAsync();
                _context.UserCoupons.RemoveRange(userCoupons);

                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Coupon deleted successfully";
                return RedirectToAction("Index");
            }

            [HttpGet]
            public async Task<IActionResult> AssignUsers(int id)
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                    return NotFound();

                if (coupon.IsPublic)
                {
                    TempData["ErrorMessage"] = "Cannot assign users to public coupons";
                    return RedirectToAction("Details", new { id });
                }

                var users = await _context.Users
                    .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.Role.RoleName == "User"))
                    .Select(u => new UserSelectViewModel
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        IsSelected = u.UserCoupons.Any(uc => uc.CouponId == id && !uc.IsUsed)
                    })
                    .ToListAsync();

                var model = new AssignUsersViewModel
                {
                    CouponId = id,
                    CouponCode = coupon.Code,
                    Users = users
                };

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> AssignUsers(AssignUsersViewModel model)
            {
                var coupon = await _context.Coupons.FindAsync(model.CouponId);
                if (coupon == null)
                    return NotFound();

                if (coupon.IsPublic)
                {
                    TempData["ErrorMessage"] = "Cannot assign users to public coupons";
                    return RedirectToAction("Details", new { id = model.CouponId });
                }

                // Remove existing assignments
                var existingAssignments = await _context.UserCoupons
                    .Where(uc => uc.CouponId == model.CouponId)
                    .ToListAsync();
                _context.UserCoupons.RemoveRange(existingAssignments);

                // Add new assignments
                if (model.SelectedUserIds?.Any() == true)
                {
                    var userCoupons = model.SelectedUserIds.Select(userId => new UserCoupon
                    {
                        UserId = userId,
                        CouponId = model.CouponId,
                        IsUsed = false
                    }).ToList();

                    _context.UserCoupons.AddRange(userCoupons);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Coupon assigned to {model.SelectedUserIds?.Count ?? 0} users successfully";
                return RedirectToAction("Details", new { id = model.CouponId });
            }

            private int GetCurrentUserId()
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out int userId) ? userId : 0;
            }
        }
    
}
