using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TestimonialController : Controller
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListJson()
        {
            var data = await _testimonialService.GetAllTestimonialsAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _testimonialService.GetTestimonialByIdAsync(id);
            return data == null ? Json(new { success = false }) : Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Testimonial model, IFormFile? ImageFile)
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadedImagePath = await FileHelper.UploadImageAsync(ImageFile, webRootPath, "uploads/testimonials");
                if (!string.IsNullOrEmpty(uploadedImagePath))
                    model.ImageUrl = uploadedImagePath;

                var result = await _testimonialService.SaveTestimonialAsync(model);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }

                return Json(new { success = true, message = result.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _testimonialService.DeleteTestimonialAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}