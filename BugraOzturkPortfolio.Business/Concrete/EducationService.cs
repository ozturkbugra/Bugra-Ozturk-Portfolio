using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class EducationService : IEducationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EducationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Education>> GetAllEducationsAsync()
        {
            var repo = _unitOfWork.GetRepository<Education>();
            var all = await repo.GetAllAsync();
            return all.Where(x => !x.IsDeleted).OrderByDescending(x => x.StartDate).ToList();
        }

        public async Task<Education?> GetEducationByIdAsync(Guid id) =>
            await _unitOfWork.GetRepository<Education>().GetByIdAsync(id);

        public async Task<(bool Success, string Message)> SaveEducationAsync(Education model)
        {
            if (string.IsNullOrEmpty(model.InstitutionName) || string.IsNullOrEmpty(model.Degree))
                return (false, "Kurum adı ve derece alanlarını doldurunuz!");

            var repo = _unitOfWork.GetRepository<Education>();

            if (model.Id == Guid.Empty || model.Id == default)
            {
                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Eğitim bilgisi başarıyla eklendi.");
            }
            else
            {
                var exist = await repo.GetByIdAsync(model.Id);
                if (exist == null) return (false, "Güncellenecek kayıt bulunamadı!");

                exist.InstitutionName = model.InstitutionName;
                exist.Degree = model.Degree;
                exist.FieldOfStudy = model.FieldOfStudy;
                exist.Description = model.Description; 
                exist.StartDate = model.StartDate;
                exist.EndDate = model.EndDate;
                exist.UpdatedDate = DateTime.UtcNow;

                repo.Update(exist);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Eğitim bilgisi güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteEducationAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Education>();
            var exist = await repo.GetByIdAsync(id);

            if (exist == null)
                return (false, "Kayıt bulunamadı!");
            exist.IsDeleted = true;

            repo.Update(exist);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Eğitim bilgisi arşivlendi (Soft Delete).");
        }
    }
}