using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            var repo = _unitOfWork.GetRepository<Project>();
            var projects = await repo.GetAllAsync();
            return projects.ToList();
        }

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Project>();
            return await repo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> SaveProjectAsync(Project model)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ShortDescription) || string.IsNullOrEmpty(model.FullDescription))
                return (false, "Lütfen proje başlığı, kısa açıklama ve detaylı açıklama alanlarını doldurunuz!");

            var repo = _unitOfWork.GetRepository<Project>();

            if (model.Id == Guid.Empty)
            {
                // Yeni kayıt (Ekleme) durumu - ID ataması BaseEntity'ye emanet
                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Proje başarıyla eklendi.");
            }
            else
            {
                // Mevcut kayıt (Güncelleme) durumu
                var existProject = await repo.GetByIdAsync(model.Id);
                if (existProject == null)
                    return (false, "Güncellenecek proje bulunamadı!");

                existProject.Title = model.Title;
                existProject.ShortDescription = model.ShortDescription;
                existProject.FullDescription = model.FullDescription;
                existProject.Client = model.Client;
                existProject.ProjectDate = model.ProjectDate;
                existProject.ProjectUrl = model.ProjectUrl;
                existProject.GithubUrl = model.GithubUrl;
                existProject.Order = model.Order;

                // Eğer arayüzden yeni bir kapak resmi yüklendiyse yolunu güncelle, boşsa eskisi kalsın
                if (!string.IsNullOrEmpty(model.CoverImageUrl))
                    existProject.CoverImageUrl = model.CoverImageUrl;

                repo.Update(existProject);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Proje başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteProjectAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Project>();
            var existProject = await repo.GetByIdAsync(id);

            if (existProject == null)
                return (false, "Silinecek proje bulunamadı!");

            repo.Delete(existProject);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Proje başarıyla silindi.");
        }
    }
}