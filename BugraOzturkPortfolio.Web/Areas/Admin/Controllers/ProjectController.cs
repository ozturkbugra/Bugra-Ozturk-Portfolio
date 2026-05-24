using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Web.Helpers;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectsJson()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Json(projects);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Proje bulunamadı!" });
            }
            return Json(new { success = true, data = project });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Project model, IFormFile? CoverImageFile, List<IFormFile>? ProjectImagesFiles, List<Guid> SelectedCategoryIds)
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadedPath = await FileHelper.UploadImageAsync(CoverImageFile, webRootPath, "uploads/projects");
                if (!string.IsNullOrEmpty(uploadedPath))
                {
                    model.CoverImageUrl = uploadedPath;
                }

                var galleryPaths = await FileHelper.UploadMultipleImagesAsync(ProjectImagesFiles, webRootPath, "uploads/projects/gallery");

                var result = await _projectService.SaveProjectAsync(model, SelectedCategoryIds, galleryPaths);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }

                return Json(new { success = true, message = result.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Sistem hatası oluştu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}