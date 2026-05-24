using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class HeadViewComponent : ViewComponent
    {
        private readonly ISiteSettingService _siteSettingService;

        public HeadViewComponent(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settingData = await _siteSettingService.GetSiteSettingAsync();
            return View(settingData);
        }
    }
}