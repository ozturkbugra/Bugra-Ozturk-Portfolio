using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class HeroViewComponent : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public HeroViewComponent(IAboutService aboutService)
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