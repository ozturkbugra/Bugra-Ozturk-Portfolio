using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SiteScriptController : Controller
    {
        private readonly ISiteScriptService _siteScriptService;

        public SiteScriptController(ISiteScriptService siteScriptService)
        {
            _siteScriptService = siteScriptService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListJson()
        {
            var data = await _siteScriptService.GetAllScriptsAsync();
            var safeData = data.Select(s => new
            {
                s.Id,
                s.Name,
                s.Url,
                s.Code,
                Position = s.Position.ToString(),
                s.IsActive
            }).ToList();

            return Json(safeData);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var script = await _siteScriptService.GetScriptByIdAsync(id);
            if (script == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                data = new
                {
                    script.Id,
                    script.Name,
                    script.Url,
                    script.Code,
                    Position = (int)script.Position,
                    script.IsActive
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(SiteScript model)
        {
            try
            {
                var (success, message) = await _siteScriptService.SaveScriptAsync(model);
                return Json(new { success, message });
            }
            catch
            {
                return Json(new { success = false, message = "İşlem sırasında teknik bir hata oluştu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var (success, message) = await _siteScriptService.ToggleStatusAsync(id);
            return Json(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, message) = await _siteScriptService.DeleteScriptAsync(id);
            return Json(new { success, message });
        }
    }
}