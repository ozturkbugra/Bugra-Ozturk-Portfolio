using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Web.Helpers;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var aboutData = await _aboutService.GetAboutDataAsync();
            return View(aboutData);
        }

        [HttpPost]
        public async Task<IActionResult> Save(About model, IFormFile? ImageFile, IFormFile? CvFile)
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadedImagePath = await FileHelper.UploadImageAsync(ImageFile, webRootPath, "uploads/images");
                if (!string.IsNullOrEmpty(uploadedImagePath)) model.ImageUrl = uploadedImagePath;

                var uploadedCvPath = await FileHelper.UploadImageAsync(CvFile, webRootPath, "uploads/docs");
                if (!string.IsNullOrEmpty(uploadedCvPath)) model.CvUrl = uploadedCvPath;

                var result = await _aboutService.SaveAboutAsync(model);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }

                return Json(new { success = true, message = result.Message, newId = model.Id });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu!" });
            }
        }
    }
}