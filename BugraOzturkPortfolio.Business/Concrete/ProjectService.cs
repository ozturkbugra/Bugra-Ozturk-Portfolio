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
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();

            var project = await projectRepo.GetByIdAsync(id);

            if (project != null)
            {
                // 1. Resimleri getir ve JSON sonsuz döngüsünü engellemek için Project referansını kopar
                var allImages = await imageRepo.GetAllAsync();
                var projectImages = allImages.Where(x => x.ProjectId == id).OrderBy(x => x.Order).ToList();
                foreach (var img in projectImages) { img.Project = null; }
                project.ProjectImages = projectImages;

                // 2. Kategorileri getir ve döngüyü engelle
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
            var projectRepo = _unitOfWork.GetRepository<Project>();
            var mappingRepo = _unitOfWork.GetRepository<ProjectCategoryMapping>();
            var imageRepo = _unitOfWork.GetRepository<ProjectImage>();

            // 1. Projenin varlığını kontrol et
            var existProject = await projectRepo.GetByIdAsync(id);
            if (existProject == null)
                return (false, "Silinecek proje bulunamadı!");

            // Fiziksel dosyaları silebilmek için wwwroot yolunu belirle
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // 2. Projeye ait tüm galeri resimlerini veritabanından çek ve hem fiziksel hem DB'den sil
            var allImages = await imageRepo.GetAllAsync();
            var projectImages = allImages.Where(x => x.ProjectId == id).ToList();

            foreach (var img in projectImages)
            {
                // Fiziksel dosyayı sunucudan sil
                if (!string.IsNullOrEmpty(img.ImageUrl))
                {
                    var fullPath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                // Veritabanı kaydını sil
                imageRepo.Delete(img);
            }

            // 3. Projeye ait tüm kategori eşleşmelerini sil
            var allMappings = await mappingRepo.GetAllAsync();
            var projectMappings = allMappings.Where(x => x.ProjectId == id).ToList();
            foreach (var mapping in projectMappings)
            {
                mappingRepo.Delete(mapping);
            }

            // 4. Projenin ana kapak resmini fiziksel olarak sunucudan sil
            if (!string.IsNullOrEmpty(existProject.CoverImageUrl))
            {
                var coverFullPath = Path.Combine(webRootPath, existProject.CoverImageUrl.TrimStart('/'));
                if (File.Exists(coverFullPath))
                {
                    File.Delete(coverFullPath);
                }
            }

            // 5. Ana projeyi veritabanından sil ve tek bir transaction olarak kaydet
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

            // Son sırayı bulup bir artırıyoruz otomatik sıralama için
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

            // 1. Önce sunucudaki fiziksel dosyayı imha ediyoruz diski şişirmemek için
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!string.IsNullOrEmpty(img.ImageUrl))
            {
                var fullPath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            // 2. Sonra veritabanı kaydını uçuruyoruz
            imageRepo.Delete(img);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Görsel başarıyla silindi.");
        }

        public async Task<List<ProjectFeature>> GetProjectFeaturesAsync(Guid projectId)
        {
            var repo = _unitOfWork.GetRepository<ProjectFeature>();
            var features = await repo.GetAllAsync();

            var projectFeatures = features.Where(x => x.ProjectId == projectId).OrderBy(x => x.Order).ToList();

            // JSON Sonsuz Döngü hatasını engellemek için ana proje bağını koparıyoruz
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

            // İkon boş geldiyse patlamasın, jilet gibi standart bir ikon ata
            if (string.IsNullOrEmpty(feature.IconClass))
                feature.IconClass = "bi bi-check2-circle";

            var repo = _unitOfWork.GetRepository<ProjectFeature>();

            if (feature.Id == Guid.Empty || feature.Id == default)
            {
                // YENİ EKLEME: Eğer sıra numarası girilmediyse, en son sırayı bulup arkasına ekle
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
                // GÜNCELLEME MODU
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