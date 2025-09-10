using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopForHome.Services;
using ShopForHome.ViewModels;

namespace ShopForHome.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetUsersAsync(page, pageSize);
                return View(users);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading users.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Roles = await _userService.GetRolesAsync();
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading roles.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    ViewBag.Roles = await _userService.GetRolesAsync();
                }
                catch
                {
                    ViewBag.Roles = new List<object>();
                }
                return View(model);
            }

            try
            {
                var result = await _userService.CreateUserAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    ViewBag.Roles = await _userService.GetRolesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception here
                ModelState.AddModelError("", "An error occurred while creating the user.");
                try
                {
                    ViewBag.Roles = await _userService.GetRolesAsync();
                }
                catch
                {
                    ViewBag.Roles = new List<object>();
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                var roles = await _userService.GetRolesAsync();
                var selectedRoleIds = roles.Where(r => user.Roles.Contains(r.RoleName)).Select(r => r.RoleId).ToList();

                var model = new UpdateUserViewModel
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    RoleIds = selectedRoleIds
                };

                ViewBag.Roles = roles;
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading user data.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    ViewBag.Roles = await _userService.GetRolesAsync();
                }
                catch
                {
                    ViewBag.Roles = new List<object>();
                }
                return View(model);
            }

            try
            {
                var result = await _userService.UpdateUserAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    ViewBag.Roles = await _userService.GetRolesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception here
                ModelState.AddModelError("", "An error occurred while updating the user.");
                try
                {
                    ViewBag.Roles = await _userService.GetRolesAsync();
                }
                catch
                {
                    ViewBag.Roles = new List<object>();
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (result.Success)
                    TempData["SuccessMessage"] = result.Message;
                else
                    TempData["ErrorMessage"] = result.Message;
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["ErrorMessage"] = "An error occurred while loading user details.";
                return RedirectToAction("Index");
            }
        }
    }
}
