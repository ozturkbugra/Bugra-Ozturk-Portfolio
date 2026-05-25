using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class LayoutScriptViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
