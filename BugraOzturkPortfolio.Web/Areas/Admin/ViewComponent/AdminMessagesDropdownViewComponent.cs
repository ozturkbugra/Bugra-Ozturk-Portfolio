using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.ViewComponents
{
    [Area("Admin")]
    public class AdminMessagesDropdownViewComponent : ViewComponent
    {
        private readonly IContactMessageService _messageService;

        public AdminMessagesDropdownViewComponent(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allMessages = await _messageService.GetAllMessagesAsync();
            var unreadMessages = allMessages.Where(x => !x.IsRead).ToList();
            return View(unreadMessages);
        }
    }
}