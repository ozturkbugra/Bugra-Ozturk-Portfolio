using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return Json(new { success = false, message = "Kullanıcı adı ve şifre boş bırakılamaz!" });

            var (success, message, user) = await _authService.LoginAsync(username, password);
            if (!success || user == null)
                return Json(new { success = false, message });

            if (user.IsTwoFactorEnabled)
            {
                // 2FA adımı için kullanıcının claims bilgilerini TempData'ya yedekliyoruz
                TempData["TwoFactorUserId"] = user.Id.ToString();
                TempData["TwoFactorUsername"] = user.Username;
                TempData["TwoFactorEmail"] = user.Email;
                TempData["TwoFactorFirstName"] = user.FirstName;
                TempData["TwoFactorLastName"] = user.LastName;
                TempData["RememberMe"] = rememberMe;
                return Json(new { success = true, requiresTwoFactor = true });
            }

            await SignInUserAsync(user.Id.ToString(), user.Username, user.Email, user.FirstName, user.LastName, rememberMe);
            return Json(new { success = true, requiresTwoFactor = false });
        }

        [HttpGet]
        public IActionResult Verify2FA()
        {
            if (TempData["TwoFactorUserId"] == null)
                return RedirectToAction("Login");

            KeepTwoFactorTempData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify2FA(string code)
        {
            if (TempData["TwoFactorUserId"] == null)
                return Json(new { success = false, message = "Oturum süresi doldu, lütfen tekrar giriş yapın." });

            string userIdStr = TempData["TwoFactorUserId"].ToString()!;
            string username = TempData["TwoFactorUsername"]?.ToString() ?? "admin";
            string email = TempData["TwoFactorEmail"]?.ToString() ?? "bugra@example.com";
            string firstName = TempData["TwoFactorFirstName"]?.ToString() ?? "Buğra";
            string lastName = TempData["TwoFactorLastName"]?.ToString() ?? "Öztürk";
            bool rememberMe = TempData["RememberMe"] != null && (bool)TempData["RememberMe"];

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                KeepTwoFactorTempData();
                return Json(new { success = false, message = "Lütfen 6 haneli kodu eksikosiz giriniz." });
            }

            Guid userId = Guid.Parse(userIdStr);
            bool isValid = await _authService.VerifyTwoFactorCodeAsync(userId, code);

            if (!isValid)
            {
                KeepTwoFactorTempData();
                return Json(new { success = false, message = "Girdiğiniz kod hatalı veya süresi dolmuş!" });
            }

            // Doğrulama başarılı, TempData temizlenebilir
            ClearTwoFactorTempData();

            await SignInUserAsync(userIdStr, username, email, firstName, lastName, rememberMe);

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async Task SignInUserAsync(string userId, string username, string email, string firstName, string lastName, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Surname, lastName),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                AllowRefresh = true
            };

            if (rememberMe)
            {
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365);
            }
            else
            {
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2);
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private void KeepTwoFactorTempData()
        {
            TempData.Keep("TwoFactorUserId");
            TempData.Keep("TwoFactorUsername");
            TempData.Keep("TwoFactorEmail");
            TempData.Keep("TwoFactorFirstName");
            TempData.Keep("TwoFactorLastName");
            TempData.Keep("RememberMe");
        }

        private void ClearTwoFactorTempData()
        {
            TempData.Remove("TwoFactorUserId");
            TempData.Remove("TwoFactorUsername");
            TempData.Remove("TwoFactorEmail");
            TempData.Remove("TwoFactorFirstName");
            TempData.Remove("TwoFactorLastName");
            TempData.Remove("RememberMe");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Lütfen e-posta adresinizi giriniz." });

            var (success, message, token) = await _authService.GeneratePasswordResetTokenAsync(email);
            if (!success || token == null)
                return Json(new { success = false, message });

            var resetLink = $"{Request.Scheme}://{Request.Host}/Admin/Auth/ResetPassword?email={UrlEncode(email)}&token={token}";

            try
            {
                await _emailService.SendResetPasswordEmailAsync(email, resetLink);
                return Json(new { success = true, message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi." });
            }
            catch
            {
                return Json(new { success = false, message = "E-posta gönderilirken teknik bir hata oluştu!" });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                return Json(new { success = false, message = "Şifre alanları boş bırakılamaz." });

            if (password != confirmPassword)
                return Json(new { success = false, message = "Şifreler birbiriyle uyuşmuyor!" });

            var (success, message) = await _authService.ResetPasswordAsync(email, token, password);
            return Json(new { success, message });
        }

        private string UrlEncode(string value) => System.Net.WebUtility.UrlEncode(value);
    }
}