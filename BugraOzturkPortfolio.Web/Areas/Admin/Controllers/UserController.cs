using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListJson()
        {
            var data = await _userService.GetAllUsersAsync();

            var safeData = data.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FirstName,
                u.LastName,
                u.IsTwoFactorEnabled
            }).ToList();

            return Json(safeData);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                data = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(User model, string? plainPassword)
        {
            try
            {
                var (success, message) = await _userService.SaveUserAsync(model, plainPassword);
                return Json(new { success, message });
            }
            catch
            {
                return Json(new { success = false, message = "İşlem sırasında teknik bir hata oluştu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            return Json(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> Reset2FA(Guid id)
        {
            var (success, message) = await _userService.ResetUserTwoFactorAsync(id);
            return Json(new { success, message });
        }
    }
}