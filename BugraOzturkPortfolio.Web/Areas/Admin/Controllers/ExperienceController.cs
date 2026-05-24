using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExperienceController : Controller
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetListJson()
            => Json(await _experienceService.GetAllExperiencesAsync());

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _experienceService.GetExperienceByIdAsync(id);
            return data == null ? Json(new { success = false }) : Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Experience model)
        {
            var result = await _experienceService.SaveExperienceAsync(model);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _experienceService.DeleteExperienceAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}