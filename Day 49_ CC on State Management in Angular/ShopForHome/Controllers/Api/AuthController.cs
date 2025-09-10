using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.Services;
using ShopForHome.ViewModels;

namespace ShopForHome.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Data = null
                });
            }

            try
            {
                var result = await _authService.LoginAsync(model.Email, model.Password);

                if (!result.Success)
                {
                    return Unauthorized(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = null
                    });
                }

                var roles = result.User.UserRoles?.Select(ur => ur.Role?.RoleName).Where(r => !string.IsNullOrEmpty(r)).ToList() ?? new List<string>();
                var token = _authService.GenerateJwtToken(result.User, roles);

                var response = new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new LoginResponse
                    {
                        Token = token,
                        Email = result.User.Email,
                        FullName = result.User.FullName,
                        Roles = roles
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Data = null
                });
            }

            try
            {
                var result = await _authService.RegisterAsync(model.FullName, model.Email, model.Password, "User");

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = result.Message,
                    Data = "Registration successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Data = null
                });
            }
        }

        [HttpPost("logout")]
        [Authorize(Policy = "ApiPolicy")]
        public ActionResult<ApiResponse<string>> Logout()
        {
            // In JWT, logout is handled on client side by removing token
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Logout successful",
                Data = null
            });
        }
    }
}
