using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IExperienceService
    {
        Task<List<Experience>> GetAllExperiencesAsync();
        Task<Experience?> GetExperienceByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveExperienceAsync(Experience model);
        Task<(bool Success, string Message)> DeleteExperienceAsync(Guid id);
    }
}