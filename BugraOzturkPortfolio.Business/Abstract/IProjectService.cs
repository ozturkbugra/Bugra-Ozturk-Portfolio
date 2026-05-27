using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveProjectAsync(Project model, List<Guid> selectedCategoryIds, List<string> galleryImagePaths);
        Task<(bool Success, string Message)> DeleteProjectAsync(Guid id);
        Task<(bool Success, string Message)> AddProjectGalleryImageAsync(Guid projectId, string imagePath);
        Task<(bool Success, string Message)> DeleteProjectGalleryImageAsync(Guid imageId);
        Task<List<ProjectFeature>> GetProjectFeaturesAsync(Guid projectId);
        Task<(bool Success, string Message)> SaveProjectFeatureAsync(ProjectFeature feature);
        Task<(bool Success, string Message)> DeleteProjectFeatureAsync(Guid featureId);

        Task<List<Project>> GetAllProjectsWithRelationsAsync();

        Task<Project?> GetProjectBySlugWithRelationsAsync(string slug);

        Task<(bool Success, string Message)> UpdateProjectOrdersAsync(List<Guid> projectIds);
    }
}