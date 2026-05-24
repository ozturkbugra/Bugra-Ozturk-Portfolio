using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IServiceService
    {
        Task<List<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveServiceAsync(Service model);
        Task<(bool Success, string Message)> DeleteServiceAsync(Guid id);
    }
}