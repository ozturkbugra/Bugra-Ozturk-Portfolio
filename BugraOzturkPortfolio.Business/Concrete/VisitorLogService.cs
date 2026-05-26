using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System;
using System.Collections.Generic;
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
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var today = nowInTurkey.Date; 

            string rawData = $"{ipAddress}_{userAgent}_{today:yyyyMMdd}";
            string hashedData = ComputeSha256Hash(rawData);

            var repo = _unitOfWork.GetRepository<VisitorLog>();

            bool alreadyLogged = await repo.AnyAsync(x => x.VisitorHash == hashedData && x.VisitDate == today);

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
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            return await repo.CountAsync(x => x.VisitDate == nowInTurkey.Date && !x.IsDeleted);
        }

        public async Task<int> GetPeriodicVisitorsCountAsync(string period)
        {
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var repo = _unitOfWork.GetRepository<VisitorLog>();
            var targetDate = nowInTurkey.Date;

            return period.ToLower() switch
            {
                "daily" => await repo.CountAsync(x => !x.IsDeleted && x.VisitDate == targetDate),
                "weekly" => await repo.CountAsync(x => !x.IsDeleted && x.VisitDate >= targetDate.AddDays(-7)),
                "monthly" => await repo.CountAsync(x => !x.IsDeleted && x.VisitDate >= targetDate.AddDays(-30)),
                "yearly" => await repo.CountAsync(x => !x.IsDeleted && x.VisitDate.Year == targetDate.Year),
                _ => await repo.CountAsync(x => !x.IsDeleted)
            };
        }

        public async Task<Dictionary<string, int>> GetLastWeekVisitorHistoryAsync()
        {
            var nowInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);
            var repo = _unitOfWork.GetRepository<VisitorLog>();
            var startDate = nowInTurkey.Date.AddDays(-6);

            var logs = await repo.GetWhereAsync(x => !x.IsDeleted && x.VisitDate >= startDate);

            var history = new Dictionary<string, int>();
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = nowInTurkey.Date.AddDays(-i);
                var count = logs.Count(x => x.VisitDate == targetDate);
                history.Add(targetDate.ToString("dd.MM (ddd)"), count);
            }

            return history;
        }

        public async Task<Dictionary<string, int>> GetTopFiveMostVisitedDaysAsync()
        {
            var repo = _unitOfWork.GetRepository<VisitorLog>();

            var logs = await repo.GetAllAsync();
            var topDays = logs
                .Where(x => !x.IsDeleted)
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
            var logs = await repo.GetWhereAsync(x => !x.IsDeleted);

            return logs
                .GroupBy(x => x.VisitDate)
                .ToDictionary(
                    g => g.Key.ToString("yyyy-MM-dd"),
                    g => g.Count()
                );
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
    }
}