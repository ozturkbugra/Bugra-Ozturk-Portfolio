using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IAboutService
    {
        Task<About?> GetAboutDataAsync();
        Task<(bool Success, string Message)> SaveAboutAsync(About model);
        Task<(bool Success, string Message)> DeleteCvFileAsync(Guid id, string webRootPath);
    }
}