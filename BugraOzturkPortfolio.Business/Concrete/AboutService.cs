using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class AboutService : IAboutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AboutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<About?> GetAboutDataAsync()
        {
            var aboutList = await _unitOfWork.GetRepository<About>().GetAllAsync();
            return aboutList.FirstOrDefault();
        }

        public async Task<(bool Success, string Message)> SaveAboutAsync(About model)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Description))
                return (false, "Lütfen gerekli alanları doldurunuz!");

            var repo = _unitOfWork.GetRepository<About>();

            if (model.Id == Guid.Empty)
            {
                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Bilgiler başarıyla eklendi.");
            }
            else
            {
                var existData = await repo.GetByIdAsync(model.Id);
                if (existData == null) return (false, "Güncellenecek kayıt bulunamadı!");

                existData.Title = model.Title;
                existData.Description = model.Description;
                existData.Email = model.Email;
                existData.Phone = model.Phone;
                existData.Address = model.Address;
                existData.GithubUrl = model.GithubUrl;
                existData.TwitterUrl = model.TwitterUrl;
                existData.FacebookUrl = model.FacebookUrl;
                existData.InstagramUrl = model.InstagramUrl;
                existData.LinkedinUrl = model.LinkedinUrl;
                existData.CvUrl = model.CvUrl;

                if (!string.IsNullOrEmpty(model.ImageUrl)) existData.ImageUrl = model.ImageUrl;

                repo.Update(existData);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Bilgiler başarıyla güncellendi.");
            }
        }
    }
}