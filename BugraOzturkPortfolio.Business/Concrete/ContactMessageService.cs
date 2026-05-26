using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactMessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ContactMessage>> GetAllMessagesAsync()
        {
            var repo = _unitOfWork.GetRepository<ContactMessage>();
            var messages = await repo.GetAllAsync();
            return messages.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public async Task<ContactMessage?> GetMessageByIdAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<ContactMessage>();
            var message = await repo.GetByIdAsync(id);
            if (message == null || message.IsDeleted) return null;
            return message;
        }

        public async Task<(bool Success, string Message)> AddMessageAsync(ContactMessage model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.PhoneNumber) ||
                string.IsNullOrWhiteSpace(model.Subject) ||
                string.IsNullOrWhiteSpace(model.Body))
            {
                return (false, "Lütfen zorunlu alanları doldurunuz!");
            }

            if (model.FullName.Length > 100 ||
                model.Email.Length > 100 ||
                model.PhoneNumber.Length > 20 ||
                model.Subject.Length > 150 ||
                model.Body.Length > 2000)
            {
                return (false, "Lütfen girdiğiniz metinlerin uzunluğunu kontrol ediniz!");
            }

            var repo = _unitOfWork.GetRepository<ContactMessage>();

            DateTime todayStart = DateTime.Now.Date;

            var allMessages = await repo.GetAllAsync();
            bool isSpam = allMessages.Any(x =>
                !x.IsDeleted &&
                x.IpAddress == model.IpAddress &&
                x.UserAgent == model.UserAgent &&
                x.CreatedDate >= todayStart 
            );

            if (isSpam)
            {
                return (false, "Bugün zaten bir mesaj gönderdiniz. Lütfen yarın tekrar deneyiniz veya doğrudan iletişim kanallarını kullanınız.");
            }

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            if (!string.IsNullOrEmpty(model.IpAddress) && model.IpAddress.Length > 45)
            {
                model.IpAddress = model.IpAddress.Substring(0, 45);
            }

            if (!string.IsNullOrEmpty(model.UserAgent) && model.UserAgent.Length > 500)
            {
                model.UserAgent = model.UserAgent.Substring(0, 500);
            }

            model.CreatedDate = DateTime.Now;

            await repo.AddAsync(model);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Mesajınız başarıyla veritabanına kaydedildi.");
        }

        public async Task<(bool Success, string Message)> MarkAsReadAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<ContactMessage>();
            var message = await repo.GetByIdAsync(id);
            if (message == null || message.IsDeleted) return (false, "Mesaj bulunamadı!");

            message.IsRead = true;
            repo.Update(message);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Mesaj okundu olarak işaretlendi.");
        }

        public async Task<(bool Success, string Message)> DeleteMessageAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<ContactMessage>();
            var message = await repo.GetByIdAsync(id);
            if (message == null || message.IsDeleted) return (false, "Silinecek mesaj bulunamadı!");

            message.IsDeleted = true;
            repo.Update(message);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Mesaj başarıyla silindi.");
        }

        public async Task<(bool Success, string Message)> DeleteMultipleMessagesAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return (false, "Silinecek herhangi bir mesaj seçilmedi!");

            var repo = _unitOfWork.GetRepository<ContactMessage>();

            foreach (var id in ids)
            {
                var message = await repo.GetByIdAsync(id);
                if (message != null && !message.IsDeleted)
                {
                    message.IsDeleted = true;
                    repo.Update(message);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return (true, $"{ids.Count} adet mesaj başarıyla silindi.");
        }
    }
}