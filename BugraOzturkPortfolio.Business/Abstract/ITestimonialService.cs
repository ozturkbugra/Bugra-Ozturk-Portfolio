using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface ITestimonialService
    {
        Task<List<Testimonial>> GetAllTestimonialsAsync();
        Task<Testimonial?> GetTestimonialByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveTestimonialAsync(Testimonial model);
        Task<(bool Success, string Message)> DeleteTestimonialAsync(Guid id);
    }
}