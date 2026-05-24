using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/ContactMessage")]
    public class ContactMessageController : Controller
    {
        private readonly IContactMessageService _messageService;

        public ContactMessageController(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            return View(messages);
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);
            if (message == null) return NotFound();

            if (!message.IsRead)
            {
                await _messageService.MarkAsReadAsync(id);
            }

            return View(message);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _messageService.DeleteMessageAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}