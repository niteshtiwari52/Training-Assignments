using ShopForHome.Models;
using ShopForHome.ViewModels;

namespace ShopForHome.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserViewModel>> GetUsersAsync(int pageNumber = 1, int pageSize = 10);
        Task<UserViewModel> GetUserByIdAsync(int userId);
        Task<(bool Success, string Message)> CreateUserAsync(CreateUserViewModel model);
        Task<(bool Success, string Message)> UpdateUserAsync(UpdateUserViewModel model);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId);
        Task<List<Role>> GetRolesAsync();
    }
}
