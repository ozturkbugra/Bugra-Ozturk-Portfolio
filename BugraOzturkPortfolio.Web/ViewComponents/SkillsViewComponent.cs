using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class SkillsViewComponent : ViewComponent
    {
        private readonly ISkillService _skillService;

        public SkillsViewComponent(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var skills = await _skillService.GetAllSkillsAsync();

            var groupedSkills = skills
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Order)
                .GroupBy(s => s.GroupName)
                .ToList();

            return View(groupedSkills);
        }
    }
}