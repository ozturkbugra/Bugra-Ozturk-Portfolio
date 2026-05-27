using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Web.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BugraOzturkPortfolio.Web.Models;
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
        public async Task<IActionResult> Inbox()
        {
            try
            {
                var messages = await _emailService.GetInboxMessagesAsync(20);
                return View(messages);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"E-posta sunucusuna bağlanırken bir hata oluştu: {ex.Message}";
                return View(new List<MailDisplayViewModel>());
            }
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendMail(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                return Json(new { success = false, message = "Lütfen alıcı (To), konu ve mesaj alanlarını eksiksiz doldurun aga!" });
            }

            try
            {
                var result = await _emailService.SendEmailAsync(to, subject, body);

                if (result)
                {
                    return Json(new { success = true, message = "E-posta başarıyla gönderildi, alıcısına mermi gibi ulaştı!" });
                }

                return Json(new { success = false, message = "E-posta gönderilirken beklenmedik bir hata oluştu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"E-posta gönderilemedi: {ex.Message}" });
            }
        }
    }
}