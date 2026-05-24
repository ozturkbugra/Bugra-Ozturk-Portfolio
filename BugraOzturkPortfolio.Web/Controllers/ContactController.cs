using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactMessageService _messageService;

        public ContactController(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("Contact/SendMessage")]
        public async Task<IActionResult> SendMessage(ContactMessage model)
        {
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Body))
            {
                return Json(new { success = false, message = "Lütfen zorunlu alanları doldurunuz!" });
            }

            try
            {
                model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
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