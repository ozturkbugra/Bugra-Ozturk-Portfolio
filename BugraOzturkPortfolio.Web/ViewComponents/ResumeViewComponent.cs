using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class ResumeViewComponent : ViewComponent
    {
        private readonly IAboutService _aboutService;
        private readonly ISkillService _skillService;
        private readonly IExperienceService _experienceService;
        private readonly IEducationService _educationService;

        public ResumeViewComponent(
            IAboutService aboutService,
            ISkillService skillService,
            IExperienceService experienceService,
            IEducationService educationService)
        {
            _aboutService = aboutService;
            _skillService = skillService;
            _experienceService = experienceService;
            _educationService = educationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about = await _aboutService.GetAboutDataAsync();

            var skills = (await _skillService.GetAllSkillsAsync())
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Order)
                .ToList();

            var experiences = (await _experienceService.GetAllExperiencesAsync())
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.StartDate)
                .ToList();

            var educations = (await _educationService.GetAllEducationsAsync())
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.StartDate)
                .ToList();

            return View((About: about, Skills: skills, Experiences: experiences, Educations: educations));
        }
    }
}