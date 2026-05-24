using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetListJson()
            => Json(await _serviceService.GetAllServicesAsync());

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _serviceService.GetServiceByIdAsync(id);
            return data == null ? Json(new { success = false }) : Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Service model)
        {
            var result = await _serviceService.SaveServiceAsync(model);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}