using ShopForHome.Models;

namespace ShopForHome.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message, User User)> RegisterAsync(string fullName, string email, string password, string roleName = "User");
        string GenerateJwtToken(User user, List<string> roles);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
