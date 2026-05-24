using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SkillController : Controller
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetListJson()
            => Json(await _skillService.GetAllSkillsAsync());

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _skillService.GetSkillByIdAsync(id);
            return data == null ? Json(new { success = false }) : Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Skill model)
        {
            var result = await _skillService.SaveSkillAsync(model);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _skillService.DeleteSkillAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}