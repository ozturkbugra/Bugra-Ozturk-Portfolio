using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeZoneInfo _turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        public VisitorLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogVisitAsync(string ipAddress, string userAgent)
        {
            var today = DateTime.Now.Date;
            string rawData = $"{ipAddress}_{userAgent}_{today:yyyyMMdd}";
            string hashedData = ComputeSha256Hash(rawData);

            var repo = _unitOfWork.GetRepository<VisitorLog>();
            var allLogs = await repo.GetAllAsync();

            bool alreadyLogged = allLogs.Any(x => x.VisitorHash == hashedData && x.VisitDate == today);

            if (!alreadyLogged)
            {
                await repo.AddAsync(new VisitorLog
                {
                    Id = Guid.NewGuid(),
                    VisitorHash = hashedData,
                    VisitDate = today,
                    CreatedDate = DateTime.UtcNow
                });
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> GetTodayUniqueVisitorsCountAsync()
        {
            var repo = _unitOfWork.GetRepository<VisitorLog>();
            var allLogs = await repo.GetAllAsync();
            return allLogs.Count(x => x.VisitDate == DateTime.UtcNow.Date && !x.IsDeleted);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<int> GetPeriodicVisitorsCountAsync(string period)
        {
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            var allLogs = (await repo.GetAllAsync()).Where(x => !x.IsDeleted).ToList();

            return period.ToLower() switch
            {
                "daily" => allLogs.Count(x => x.VisitDate == nowInTurkey.Date),
                "weekly" => allLogs.Count(x => x.VisitDate >= nowInTurkey.Date.AddDays(-7)),
                "monthly" => allLogs.Count(x => x.VisitDate >= nowInTurkey.Date.AddDays(-30)),
                "yearly" => allLogs.Count(x => x.VisitDate.Year == nowInTurkey.Year),
                _ => allLogs.Count()
            };
        }

        public async Task<Dictionary<string, int>> GetLastWeekVisitorHistoryAsync()
        {
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            var allLogs = (await repo.GetAllAsync()).Where(x => !x.IsDeleted).ToList();

            var history = new Dictionary<string, int>();

            for (int i = 6; i >= 0; i--)
            {
                var targetDate = nowInTurkey.Date.AddDays(-i);
                var count = allLogs.Count(x => x.VisitDate == targetDate);
                history.Add(targetDate.ToString("dd.MM (ddd)"), count);
            }

            return history;
        }

        public async Task<Dictionary<string, int>> GetTopFiveMostVisitedDaysAsync()
        {
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            var allLogs = (await repo.GetAllAsync()).Where(x => !x.IsDeleted).ToList();
            var topDays = allLogs
                .GroupBy(x => x.VisitDate)
                .Select(g => new { DateStr = g.Key.ToString("dd.MM.yyyy"), Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToDictionary(x => x.DateStr, x => x.Count);

            return topDays;
        }

        public async Task<Dictionary<string, int>> GetVisitorCountHistoryForCalendarAsync()
        {
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            var allLogs = (await repo.GetAllAsync()).Where(x => !x.IsDeleted).ToList();

            var calendarData = allLogs
                .GroupBy(x => x.VisitDate)
                .ToDictionary(
                    g => g.Key.ToString("yyyy-MM-dd"),
                    g => g.Count()
                );

            return calendarData;
        }
    }
}