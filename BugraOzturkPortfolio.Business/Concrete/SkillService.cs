using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class SkillService : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SkillService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return (await _unitOfWork.GetRepository<Skill>().GetAllAsync())
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.GroupName).ThenByDescending(x => x.Percentage)
                .ToList();
        }

        public async Task<Skill?> GetSkillByIdAsync(Guid id) => await _unitOfWork.GetRepository<Skill>().GetByIdAsync(id);

        public async Task<(bool Success, string Message)> SaveSkillAsync(Skill model)
        {
            var repo = _unitOfWork.GetRepository<Skill>();

            var all = await repo.GetAllAsync();
            if (all.Any(x => x.Title.ToLower() == model.Title.ToLower() && x.GroupName == model.GroupName && x.Id != model.Id && !x.IsDeleted))
                return (false, "Bu yetenek zaten bu grupta tanımlı!");

            model.UpdatedDate = DateTime.UtcNow;

            if (model.Id == Guid.Empty || model.Id == default)
            {
                model.IsDeleted = false;
                await repo.AddAsync(model);
            }
            else
            {
                var exist = await repo.GetByIdAsync(model.Id);
                if (exist == null) return (false, "Kayıt bulunamadı!");
                exist.Title = model.Title;
                exist.Percentage = model.Percentage;
                exist.GroupName = model.GroupName;
                exist.Order = model.Order;
                exist.UpdatedDate = DateTime.UtcNow;
                repo.Update(exist);
            }
            await _unitOfWork.SaveChangesAsync();
            return (true, "Yetenek kaydedildi.");
        }

        public async Task<(bool Success, string Message)> DeleteSkillAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Skill>();
            var exist = await repo.GetByIdAsync(id);
            if (exist == null) return (false, "Kayıt bulunamadı!");

            exist.IsDeleted = true;
            exist.UpdatedDate = DateTime.UtcNow;
            repo.Update(exist);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Yetenek arşivlendi.");
        }
    }
}