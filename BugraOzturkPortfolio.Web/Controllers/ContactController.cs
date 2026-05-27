using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR; 

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext; 

        public ContactController(IContactMessageService messageService, IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
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

            try
            {
                model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                model.UserAgent = Request.Headers["User-Agent"].ToString();

                var result = await _messageService.AddMessageAsync(model);

                if (result.Success)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNewMessage", new
                    {
                        id = model.Id,
                        fullName = model.FullName,
                        subject = model.Subject,
                        createdDate = DateTime.UtcNow.ToLocalTime().ToString("dd.MM HH:mm")
                    });
                }

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Mesaj iletilirken sistemsel bir hata oluştu!" });
            }
        }
    }
}