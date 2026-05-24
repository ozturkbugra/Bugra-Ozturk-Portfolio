using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class SiteScriptService : ISiteScriptService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SiteScriptService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SiteScript>> GetAllScriptsAsync()
        {
            return (await _unitOfWork.GetRepository<SiteScript>().GetAllAsync())
                .Where(s => !s.IsDeleted)
                .ToList();
        }

        public async Task<List<SiteScript>> GetActiveScriptsByPositionAsync(ScriptPosition position)
        {
            return (await _unitOfWork.GetRepository<SiteScript>().GetAllAsync())
                .Where(s => !s.IsDeleted && s.IsActive && s.Position == position)
                .ToList();
        }

        public async Task<SiteScript?> GetScriptByIdAsync(Guid id)
        {
            return await _unitOfWork.GetRepository<SiteScript>().GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> SaveScriptAsync(SiteScript model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return (false, "Script adı boş bırakılamaz!");

            if (string.IsNullOrEmpty(model.Url) && string.IsNullOrEmpty(model.Code))
                return (false, "Script için kaynak URL veya ham kod alanlarından en az biri doldurulmalıdır!");

            var repo = _unitOfWork.GetRepository<SiteScript>();

            if (model.Id == Guid.Empty)
            {
                model.IsDeleted = false;
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;

                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Script başarıyla sisteme eklendi.");
            }
            else
            {
                var existScript = await repo.GetByIdAsync(model.Id);
                if (existScript == null || existScript.IsDeleted)
                    return (false, "Güncellenecek kayıt bulunamadı!");

                existScript.Name = model.Name;
                existScript.Url = model.Url;
                existScript.Code = model.Code;
                existScript.Position = model.Position;
                existScript.IsActive = model.IsActive;
                existScript.UpdatedDate = DateTime.UtcNow;

                repo.Update(existScript);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Script başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteScriptAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<SiteScript>();
            var existScript = await repo.GetByIdAsync(id);
            if (existScript == null || existScript.IsDeleted)
                return (false, "Silinecek kayıt bulunamadı!");

            existScript.IsDeleted = true;
            existScript.UpdatedDate = DateTime.UtcNow;

            repo.Update(existScript);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Script başarıyla silindi.");
        }

        public async Task<(bool Success, string Message)> ToggleStatusAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<SiteScript>();
            var existScript = await repo.GetByIdAsync(id);
            if (existScript == null || existScript.IsDeleted)
                return (false, "Kayıt bulunamadı!");

            existScript.IsActive = !existScript.IsActive;

            repo.Update(existScript);
            await _unitOfWork.SaveChangesAsync();
            return (true, existScript.IsActive ? "Script aktif hale getirildi." : "Script devre dışı bırakıldı.");
        }
    }
}