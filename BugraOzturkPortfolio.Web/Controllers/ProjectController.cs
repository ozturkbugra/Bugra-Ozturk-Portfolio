using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Web.Models;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ICategoryService _categoryService;

        public ProjectController(IProjectService projectService, ICategoryService categoryService)
        {
            _projectService = projectService;
            _categoryService = categoryService;
        }

        [HttpGet("proje/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var project = await _projectService.GetProjectBySlugWithRelationsAsync(slug);
            if (project == null)
            {
                return RedirectToAction("Index", "Anasayfa");
            }

            var allCategories = await _categoryService.GetAllCategoriesAsync();
            var features = await _projectService.GetProjectFeaturesAsync(project.Id);

            var viewModel = new ProjectDetailViewModel
            {
                Project = project,
                Categories = allCategories.Where(c => project.CategoryMappings.Any(m => m.CategoryId == c.Id)).ToList(),
                ProjectFeatures = features.OrderBy(x => x.Order).ToList()
            };

            return View(viewModel);
        }
    }
}