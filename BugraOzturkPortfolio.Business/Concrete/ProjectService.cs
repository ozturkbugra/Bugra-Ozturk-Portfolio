using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System.Text.RegularExpressions;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.Trim().ToLower();
            text = text.Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ı", "i").Replace("ö", "o").Replace("ç", "c");
            text = Regex.Replace(text, @"[^a-z0-0\s-]", "");
            text = Regex.Replace(text, @"\s+", "-");
            text = Regex.Replace(text, @"-+", "-");
            return text.Trim('-');
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            var repo = _unitOfWork.GetRepository<Project>();
            var projects = await repo.GetAllAsync();
            return projects.ToList();
        }

        public async Task<List<Project>> GetAllProjectsWithRelationsAsync()
        {
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();

            var projects = (await projectRepo.GetAllAsync()).ToList();
            var allImages = await imageRepo.GetAllAsync();
            var allMappings = await mappingRepo.GetAllAsync(); // Ara eşleşme tablosunu çekiyoruz

            foreach (var project in projects)
            {
                // 1. Resimleri bağla
                var projectImages = allImages.Where(x => x.ProjectId == project.Id).OrderBy(x => x.Order).ToList();
                foreach (var img in projectImages) { img.Project = null; }
                project.ProjectImages = projectImages;

                // 2. Kategori eşleşmelerini bağla (Sonsuz döngüyü engellemek için Project bağını kopar)
                var projectMappings = allMappings.Where(x => x.ProjectId == project.Id).ToList();
                foreach (var map in projectMappings)
                {
                    map.Project = null;
                    map.Category = null; // Kategori nesnesini null bırakıyoruz, çünkü ID'si bize yetecek!
                }
                project.CategoryMappings = projectMappings;
            }

            return projects;
        }
        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();

            var project = await projectRepo.GetByIdAsync(id);

            if (project != null)
            {
                var allImages = await imageRepo.GetAllAsync();
                var projectImages = allImages.Where(x => x.ProjectId == id).OrderBy(x => x.Order).ToList();
                foreach (var img in projectImages) { img.Project = null; }
                project.ProjectImages = projectImages;

                var allMappings = await mappingRepo.GetAllAsync();
                var projectMappings = allMappings.Where(x => x.ProjectId == id).ToList();
                foreach (var map in projectMappings) { map.Project = null; map.Category = null; }
                project.CategoryMappings = projectMappings;
            }

            return project;
        }

        public async Task<(bool Success, string Message)> SaveProjectAsync(Project model, List<Guid> selectedCategoryIds, List<string> galleryImagePaths)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ShortDescription) || string.IsNullOrEmpty(model.FullDescription))
                return (false, "Lütfen proje başlığı, kısa açıklama ve detaylı açıklama alanlarını doldurunuz!");

            var projectRepo = _unitOfWork.GetRepository<Project>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();

            model.Slug = GenerateSlug(model.Title);

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
                existProject.Slug = model.Slug;
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
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();

            var existProject = await projectRepo.GetByIdAsync(id);
            if (existProject == null)
                return (false, "Silinecek proje bulunamadı!");

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var allImages = await imageRepo.GetAllAsync();
            var projectImages = allImages.Where(x => x.ProjectId == id).ToList();

            foreach (var img in projectImages)
            {
                if (!string.IsNullOrEmpty(img.ImageUrl))
                {
                    var fullPath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                imageRepo.Delete(img);
            }

            var allMappings = await mappingRepo.GetAllAsync();
            var projectMappings = allMappings.Where(x => x.ProjectId == id).ToList();
            foreach (var mapping in projectMappings)
            {
                mappingRepo.Delete(mapping);
            }

            if (!string.IsNullOrEmpty(existProject.CoverImageUrl))
            {
                var coverFullPath = Path.Combine(webRootPath, existProject.CoverImageUrl.TrimStart('/'));
                if (File.Exists(coverFullPath))
                {
                    File.Delete(coverFullPath);
                }
            }

            projectRepo.Delete(existProject);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Proje ve ilişkili tüm görseller başarıyla silindi.");
        }

        public async Task<(bool Success, string Message)> AddProjectGalleryImageAsync(Guid projectId, string imagePath)
        {
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var project = await projectRepo.GetByIdAsync(projectId);
            if (project == null) return (false, "Proje bulunamadı!");

            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();

            var allImages = await imageRepo.GetAllAsync();
            var maxOrder = allImages.Where(x => x.ProjectId == projectId).Select(x => x.Order).DefaultIfEmpty(0).Max();

            await imageRepo.AddAsync(new ProjectImage
            {
                ProjectId = projectId,
                ImageUrl = imagePath,
                Order = maxOrder + 1
            });

            await _unitOfWork.SaveChangesAsync();
            return (true, "Galeri resmi başarıyla eklendi.");
        }

        public async Task<(bool Success, string Message)> DeleteProjectGalleryImageAsync(Guid imageId)
        {
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();
            var img = await imageRepo.GetByIdAsync(imageId);
            if (img == null) return (false, "Silinecek görsel bulunamadı!");

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!string.IsNullOrEmpty(img.ImageUrl))
            {
                var fullPath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            imageRepo.Delete(img);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Görsel başarıyla silindi.");
        }

        public async Task<List<ProjectFeature>> GetProjectFeaturesAsync(Guid projectId)
        {
            var repo = _unitOfWork.GetRepository<ProjectFeature>();
            var features = await repo.GetAllAsync();
            var projectFeatures = features.Where(x => x.ProjectId == projectId).OrderBy(x => x.Order).ToList();

            foreach (var feature in projectFeatures)
            {
                feature.Project = null;
            }

            return projectFeatures;
        }

        public async Task<(bool Success, string Message)> SaveProjectFeatureAsync(ProjectFeature feature)
        {
            if (string.IsNullOrEmpty(feature.Title) || string.IsNullOrEmpty(feature.Description))
                return (false, "Lütfen özellik başlığı ve açıklamasını doldurunuz!");

            if (string.IsNullOrEmpty(feature.IconClass))
                feature.IconClass = "bi bi-check2-circle";

            var repo = _unitOfWork.GetRepository<ProjectFeature>();

            if (feature.Id == Guid.Empty || feature.Id == default)
            {
                if (feature.Order == 0)
                {
                    var allFeatures = await repo.GetAllAsync();
                    var maxOrder = allFeatures.Where(x => x.ProjectId == feature.ProjectId)
                                              .Select(x => x.Order)
                                              .DefaultIfEmpty(0).Max();
                    feature.Order = maxOrder + 1;
                }

                await repo.AddAsync(feature);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Proje özelliği başarıyla eklendi.");
            }
            else
            {
                var existFeature = await repo.GetByIdAsync(feature.Id);
                if (existFeature == null) return (false, "Güncellenecek özellik bulunamadı!");

                existFeature.Title = feature.Title;
                existFeature.Description = feature.Description;
                existFeature.IconClass = feature.IconClass;
                existFeature.Order = feature.Order;

                repo.Update(existFeature);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Proje özelliği başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteProjectFeatureAsync(Guid featureId)
        {
            var repo = _unitOfWork.GetRepository<ProjectFeature>();
            var feature = await repo.GetByIdAsync(featureId);
            if (feature == null) return (false, "Silinecek özellik bulunamadı!");

            repo.Delete(feature);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Proje özelliği başarıyla silindi.");
        }
    }
}