using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationController : Controller
    {
        private readonly IEducationService _educationService;
        public EducationController(IEducationService educationService) => _educationService = educationService;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetListJson() => Json(await _educationService.GetAllEducationsAsync());

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _educationService.GetEducationByIdAsync(id);
            return data == null ? Json(new { success = false }) : Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Education model)
        {
            var result = await _educationService.SaveEducationAsync(model);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _educationService.DeleteEducationAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}