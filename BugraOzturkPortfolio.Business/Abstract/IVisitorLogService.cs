using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IVisitorLogService
    {
        Task LogVisitAsync(string ipAddress, string userAgent);
        Task<int> GetTodayUniqueVisitorsCountAsync();

        Task<int> GetPeriodicVisitorsCountAsync(string period); // daily, weekly, monthly, yearly filtreleri için
        Task<Dictionary<string, int>> GetLastWeekVisitorHistoryAsync(); // Son 1 haftalık çizgi grafiği verisi
        Task<Dictionary<string, int>> GetTopFiveMostVisitedDaysAsync(); // En çok girilen 5 gün pasta dilimi verisi
    }
}