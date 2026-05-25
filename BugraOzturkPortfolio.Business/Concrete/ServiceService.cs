using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return (await _unitOfWork.GetRepository<Service>().GetAllAsync())
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Order)
                .ToList();
        }

        public async Task<Service?> GetServiceByIdAsync(Guid id) =>
            await _unitOfWork.GetRepository<Service>().GetByIdAsync(id);

        public async Task<(bool Success, string Message)> SaveServiceAsync(Service model)
        {
            var repo = _unitOfWork.GetRepository<Service>();

            var all = await repo.GetAllAsync();
            if (all.Any(x => x.Title.ToLower() == model.Title.ToLower() && x.Id != model.Id && !x.IsDeleted))
                return (false, "Bu başlıkta zaten bir hizmet mevcut!");

            model.UpdatedDate = DateTime.UtcNow;

            if (model.Id == Guid.Empty || model.Id == default)
            {
                model.IsDeleted = false;
                await repo.AddAsync(model);
            }
            else
            {
                var exist = await repo.GetByIdAsync(model.Id);
                if (exist == null) return (false, "Hizmet bulunamadı!");

                exist.Title = model.Title;
                exist.Description = model.Description;
                exist.IconClass = model.IconClass;
                exist.Order = model.Order;
                exist.UpdatedDate = model.UpdatedDate;
                exist.IsDeleted = false;
                repo.Update(exist);
            }
            await _unitOfWork.SaveChangesAsync();
            return (true, "Hizmet başarıyla kaydedildi.");
        }

        public async Task<(bool Success, string Message)> DeleteServiceAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Service>();
            var exist = await repo.GetByIdAsync(id);
            if (exist == null) return (false, "Hizmet bulunamadı!");

            exist.IsDeleted = true;
            repo.Update(exist);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Hizmet arşivlendi.");
        }
    }
}