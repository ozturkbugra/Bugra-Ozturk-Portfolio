using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.ViewComponents
{
    [Area("Admin")]
    public class AdminSidebarMessagesBadgeViewComponent : ViewComponent
    {
        private readonly IContactMessageService _messageService;

        public AdminSidebarMessagesBadgeViewComponent(IContactMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allMessages = await _messageService.GetAllMessagesAsync();
            int unreadCount = allMessages.Count(x => !x.IsRead);
            return View(unreadCount);
        }
    }
}