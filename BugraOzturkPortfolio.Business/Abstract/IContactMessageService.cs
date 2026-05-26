using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IContactMessageService
    {
        Task<List<ContactMessage>> GetAllMessagesAsync();
        Task<ContactMessage?> GetMessageByIdAsync(Guid id);
        Task<(bool Success, string Message)> AddMessageAsync(ContactMessage model);
        Task<(bool Success, string Message)> MarkAsReadAsync(Guid id);
        Task<(bool Success, string Message)> DeleteMessageAsync(Guid id);
        Task<(bool Success, string Message)> DeleteMultipleMessagesAsync(List<Guid> ids);

    }
}