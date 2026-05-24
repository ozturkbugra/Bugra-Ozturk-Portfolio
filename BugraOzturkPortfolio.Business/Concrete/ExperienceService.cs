using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class ExperienceService : IExperienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExperienceService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Experience>> GetAllExperiencesAsync()
        {
            var repo = _unitOfWork.GetRepository<Experience>();
            return (await repo.GetAllAsync())
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }

        public async Task<Experience?> GetExperienceByIdAsync(Guid id)
            => await _unitOfWork.GetRepository<Experience>().GetByIdAsync(id);

        public async Task<(bool Success, string Message)> SaveExperienceAsync(Experience model)
        {
            if (string.IsNullOrEmpty(model.CompanyName) || string.IsNullOrEmpty(model.Position))
                return (false, "Firma adı ve pozisyon alanları zorunludur!");

            var repo = _unitOfWork.GetRepository<Experience>();

            if (model.Id == Guid.Empty || model.Id == default)
            {
                model.IsDeleted = false;
                await repo.AddAsync(model);
            }
            else
            {
                var exist = await repo.GetByIdAsync(model.Id);
                if (exist == null) return (false, "Deneyim kaydı bulunamadı!");

                exist.CompanyName = model.CompanyName;
                exist.Position = model.Position;
                exist.Description = model.Description;
                exist.StartDate = model.StartDate;
                exist.EndDate = model.EndDate;
                exist.Location = model.Location;
                exist.UpdatedDate = DateTime.UtcNow;
                exist.IsDeleted = false;

                repo.Update(exist);
            }
            await _unitOfWork.SaveChangesAsync();
            return (true, "Deneyim bilgisi başarıyla kaydedildi.");
        }

        public async Task<(bool Success, string Message)> DeleteExperienceAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Experience>();
            var exist = await repo.GetByIdAsync(id);
            if (exist == null) return (false, "Kayıt bulunamadı!");

            exist.IsDeleted = true;

            repo.Update(exist);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Deneyim bilgisi başarıyla arşivlendi.");
        }
    }
}