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

        public VisitorLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogVisitAsync(string ipAddress, string userAgent)
        {
            var today = DateTime.UtcNow.Date;
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
    }
}