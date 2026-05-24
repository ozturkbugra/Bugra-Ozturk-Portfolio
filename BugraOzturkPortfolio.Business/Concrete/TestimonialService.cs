using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class TestimonialService : ITestimonialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestimonialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Testimonial>> GetAllTestimonialsAsync()
        {
            return (await _unitOfWork.GetRepository<Testimonial>().GetAllAsync())
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Order)
                .ToList();
        }

        public async Task<Testimonial?> GetTestimonialByIdAsync(Guid id)
        {
            return await _unitOfWork.GetRepository<Testimonial>().GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> SaveTestimonialAsync(Testimonial model)
        {
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Comment))
                return (false, "Lütfen gerekli alanları doldurunuz!");

            var repo = _unitOfWork.GetRepository<Testimonial>();

            if (model.Id == Guid.Empty)
            {
                model.IsDeleted = false;
                model.UpdatedDate = DateTime.UtcNow;
                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Referans başarıyla eklendi.");
            }
            else
            {
                var existData = await repo.GetByIdAsync(model.Id);
                if (existData == null) return (false, "Güncellenecek kayıt bulunamadı!");

                existData.FullName = model.FullName;
                existData.Company = model.Company;
                existData.Position = model.Position;
                existData.Comment = model.Comment;
                existData.Order = model.Order;
                existData.UpdatedDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(model.ImageUrl))
                    existData.ImageUrl = model.ImageUrl;

                repo.Update(existData);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Referans başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteTestimonialAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Testimonial>();
            var existData = await repo.GetByIdAsync(id);
            if (existData == null) return (false, "Kayıt bulunamadı!");

            existData.IsDeleted = true;

            repo.Update(existData);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Referans başarıyla silindi");
        }
    }
}