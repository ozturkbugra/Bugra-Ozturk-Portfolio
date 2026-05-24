using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using System.Linq;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class PortfolioViewComponent : ViewComponent
    {
        private readonly IProjectService _projectService;
        private readonly ICategoryService _categoryService; 

        public PortfolioViewComponent(IProjectService projectService, ICategoryService categoryService)
        {
            _projectService = projectService;
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var projects = (await _projectService.GetAllProjectsWithRelationsAsync())
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Order)
                .ToList();

            var categories = (await _categoryService.GetAllCategoriesAsync()) 
                .Where(c => !c.IsDeleted)
                .ToList();

            return View((Projects: projects, Categories: categories));
        }
    }
}