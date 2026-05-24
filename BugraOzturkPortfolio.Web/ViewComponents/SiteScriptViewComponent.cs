using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class SiteScriptViewComponent : ViewComponent
    {
        private readonly ISiteScriptService _siteScriptService;

        public SiteScriptViewComponent(ISiteScriptService siteScriptService)
        {
            _siteScriptService = siteScriptService;
        }

        public async Task<IViewComponentResult> InvokeAsync(ScriptPosition position)
        {
            var scripts = await _siteScriptService.GetActiveScriptsByPositionAsync(position);
            return View(scripts);
        }
    }
}