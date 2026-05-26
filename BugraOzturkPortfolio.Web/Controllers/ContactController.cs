using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactMessageService _messageService;

        public ContactController(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("iletisim")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "İletişim";
            return View();
        }

        [HttpPost("Contact/SendMessage")]
        public async Task<IActionResult> SendMessage(ContactMessage model, [FromForm] int? UserCaptchaAnswer)
        {
            int? correctResult = HttpContext.Session.GetInt32("CaptchaResult");

            if (correctResult == null || UserCaptchaAnswer == null || UserCaptchaAnswer != correctResult)
            {
                return Json(new { success = false, message = "Bot doğrulaması başarısız! İşlem sonucunu kontrol edin" });
            }

            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Subject) ||
                string.IsNullOrWhiteSpace(model.Body))
            {
                return Json(new { success = false, message = "Lütfen zorunlu alanları doldurunuz!" });
            }

            if (model.FullName.Length > 100 ||
                model.Email.Length > 100 ||
                model.Subject.Length > 150 ||
                model.Body.Length > 2000)
            {
                return Json(new { success = false, message = "Lütfen girdiğiniz metinlerin uzunluğunu kontrol ediniz!" });
            }

            try
            {
                model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                if (model.IpAddress.Length > 45)
                {
                    model.IpAddress = model.IpAddress.Substring(0, 45);
                }

                model.CreatedDate = DateTime.UtcNow;

                var result = await _messageService.AddMessageAsync(model);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Mesaj iletilirken sistemsel bir hata oluştu!" });
            }
        }
    }
}