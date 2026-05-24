using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class SiteSettingController : Controller
{
    private readonly ISiteSettingService _service;
    public SiteSettingController(ISiteSettingService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetSiteSettingAsync());

    [HttpPost]
    public async Task<IActionResult> Save(SiteSetting model, IFormFile? FaviconFile, IFormFile? AppleTouchFile)
    {
        try
        {
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Favicon Yükleme
            var faviconPath = await FileHelper.UploadImageAsync(FaviconFile, webRootPath, "uploads/icons");
            if (!string.IsNullOrEmpty(faviconPath)) model.FaviconUrl = faviconPath;

            // Apple Touch Icon Yükleme
            var applePath = await FileHelper.UploadImageAsync(AppleTouchFile, webRootPath, "uploads/icons");
            if (!string.IsNullOrEmpty(applePath)) model.AppleTouchIconUrl = applePath;

            var result = await _service.UpdateSiteSettingAsync(model);
            return Json(new { success = result.Success, message = result.Message });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Dosya yükleme sırasında hata oluştu!" });
        }
    }
}