using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveProjectAsync(Project model);
        Task<(bool Success, string Message)> DeleteProjectAsync(Guid id);
    }
}