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
            var repo = _unitOfWork.GetRepository<ContactMessage>();

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

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
    }
}