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

        public async Task<(bool Success, string Message)> SaveProjectAsync(Project model, List<Guid> selectedCategoryIds, List<string> galleryImagePaths)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ShortDescription) || string.IsNullOrEmpty(model.FullDescription))
                return (false, "Lütfen proje başlığı, kısa açıklama ve detaylı açıklama alanlarını doldurunuz!");

            var projectRepo = _unitOfWork.GetRepository<Project>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();

            if (model.Id == Guid.Empty || model.Id == default)
            {
                await projectRepo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();

                if (selectedCategoryIds != null && selectedCategoryIds.Any())
                {
                    foreach (var catId in selectedCategoryIds)
                    {
                        await mappingRepo.AddAsync(new ProjectCategoryMapping
                        {
                            ProjectId = model.Id,
                            CategoryId = catId
                        });
                    }
                }

                if (galleryImagePaths != null && galleryImagePaths.Any())
                {
                    foreach (var path in galleryImagePaths)
                    {
                        await imageRepo.AddAsync(new ProjectImage
                        {
                            ProjectId = model.Id,
                            ImageUrl = path
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return (true, "Proje başarıyla eklendi.");
            }
            else
            {
                var existProject = await projectRepo.GetByIdAsync(model.Id);
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

                if (!string.IsNullOrEmpty(model.CoverImageUrl))
                    existProject.CoverImageUrl = model.CoverImageUrl;

                projectRepo.Update(existProject);

                var oldMappings = await mappingRepo.GetAllAsync();
                var currentMappings = oldMappings.Where(x => x.ProjectId == model.Id).ToList();
                foreach (var mapping in currentMappings)
                {
                    mappingRepo.Delete(mapping);
                }

                if (selectedCategoryIds != null && selectedCategoryIds.Any())
                {
                    foreach (var catId in selectedCategoryIds)
                    {
                        await mappingRepo.AddAsync(new ProjectCategoryMapping
                        {
                            ProjectId = model.Id,
                            CategoryId = catId
                        });
                    }
                }

                if (galleryImagePaths != null && galleryImagePaths.Any())
                {
                    foreach (var path in galleryImagePaths)
                    {
                        await imageRepo.AddAsync(new ProjectImage
                        {
                            ProjectId = model.Id,
                            ImageUrl = path
                        });
                    }
                }

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