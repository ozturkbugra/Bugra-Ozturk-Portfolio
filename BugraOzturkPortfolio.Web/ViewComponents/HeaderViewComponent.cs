using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public HeaderViewComponent(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var aboutData = await _aboutService.GetAboutDataAsync();
            return View(aboutData);
        }
    }
}