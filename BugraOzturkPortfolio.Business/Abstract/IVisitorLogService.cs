using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IVisitorLogService
    {
        Task LogVisitAsync(string ipAddress, string userAgent);
        Task<int> GetTodayUniqueVisitorsCountAsync();
    }
}