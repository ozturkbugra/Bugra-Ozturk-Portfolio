using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SetupTwoFactor()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Kullanıcı oturumu bulunamadı!" });

            Guid userId = Guid.Parse(userIdStr);
            var (success, secretKey, qrCodeUrl) = await _authService.GenerateTwoFactorSecretAsync(userId);

            if (!success)
                return Json(new { success = false, message = "2FA anahtarı üretilirken bir hata oluştu." });

            return Json(new { success = true, secretKey, qrCodeUrl });
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactor(string code)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Kullanıcı oturumu bulunamadı!" });

            if (string.IsNullOrEmpty(code) || code.Length != 6)
                return Json(new { success = false, message = "Lütfen 6 haneli doğrulama kodunu giriniz." });

            Guid userId = Guid.Parse(userIdStr);

            bool isCodeValid = await _authService.VerifyTwoFactorCodeAsync(userId, code);
            if (!isCodeValid)
                return Json(new { success = false, message = "Girdiğiniz kod hatalı. Kurulum başarısız!" });

            var (success, message) = await _authService.ToggleTwoFactorAsync(userId, true);

            return Json(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Kullanıcı oturumu bulunamadı!" });

            Guid userId = Guid.Parse(userIdStr);

            var (success, message) = await _authService.ToggleTwoFactorAsync(userId, false);

            return Json(new { success, message });
        }
    }
}