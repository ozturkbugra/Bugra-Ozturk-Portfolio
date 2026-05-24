using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class ServicesViewComponent : ViewComponent
    {
        private readonly IServiceService _serviceService;

        public ServicesViewComponent(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var services = await _serviceService.GetAllServicesAsync();

            var activeServices = services
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Order)
                .ToList();

            return View(activeServices);
        }
    }
}