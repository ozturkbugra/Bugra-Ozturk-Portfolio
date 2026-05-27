using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Web.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin/Email")]
    public class EmailController : Controller
    {
        private readonly EmailManagementService _emailService;

        public EmailController(EmailManagementService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Inbox([FromQuery] string folder = "INBOX")
        {
            try
            {
                var messages = await _emailService.GetMessagesFromFolderAsync(folder, 20);
                ViewBag.CurrentFolder = folder; 
                return View(messages);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"E-posta listesi alınamadı: {ex.Message}";
                ViewBag.CurrentFolder = folder;
                return View(new System.Collections.Generic.List<BugraOzturkPortfolio.Web.Models.MailDisplayViewModel>());
            }
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendMail(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                return Json(new { success = false, message = "Lütfen gerekli alanları eksiksiz doldurun aga!" });
            }

            try
            {
                var result = await _emailService.SendEmailAsync(to, subject, body);
                if (result) return Json(new { success = true, message = "E-posta mermi gibi gönderildi ve kaydedildi!" });
                return Json(new { success = false, message = "E-posta gönderilirken hata oluştu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}