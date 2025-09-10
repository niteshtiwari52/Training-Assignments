using Microsoft.EntityFrameworkCore;
using ShopForHome.Data;
using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public class UserService : IUserService
    {
        private readonly ShopForHomeDbContext _context;
        private readonly IAuthService _authService;

        public UserService(ShopForHomeDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<PagedResult<UserViewModel>> GetUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<UserViewModel>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<UserViewModel> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            return new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(CreateUserViewModel model)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return (false, "Email already exists");

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = _authService.HashPassword(model.Password),
                IsActive = model.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            foreach (var roleId in model.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = roleId
                };
                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();
            return (true, "User created successfully");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(UpdateUserViewModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null)
                return (false, "User not found");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != model.UserId);
            if (existingUser != null)
                return (false, "Email already exists");

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.IsActive = model.IsActive;

            // Update roles
            _context.UserRoles.RemoveRange(user.UserRoles);
            foreach (var roleId in model.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = roleId
                };
                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();
            return (true, "User updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return (true, "User deleted successfully");
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
