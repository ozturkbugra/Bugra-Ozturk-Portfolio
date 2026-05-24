using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesJson()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Json(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }
            return Json(new { success = true, data = category });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Category model)
        {
            try
            {
                var result = await _categoryService.SaveCategoryAsync(model);

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
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}