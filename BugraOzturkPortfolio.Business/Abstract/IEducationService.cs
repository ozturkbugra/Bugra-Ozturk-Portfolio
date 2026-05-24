using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IEducationService
    {
        Task<List<Education>> GetAllEducationsAsync();
        Task<Education?> GetEducationByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveEducationAsync(Education model);
        Task<(bool Success, string Message)> DeleteEducationAsync(Guid id);
    }
}