using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class TestimonialsViewComponent : ViewComponent
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialsViewComponent(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var testimonials = await _testimonialService.GetAllTestimonialsAsync();

            var activeTestimonials = testimonials
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ToList();

            return View(activeTestimonials);
        }
    }
}