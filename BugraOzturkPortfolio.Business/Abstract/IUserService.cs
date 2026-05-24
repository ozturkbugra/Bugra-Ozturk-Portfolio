using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<(bool Success, string Message)> SaveUserAsync(User model, string? plainPassword = null);
        Task<(bool Success, string Message)> DeleteUserAsync(Guid id);
        Task<(bool Success, string Message)> ResetUserTwoFactorAsync(Guid id);
    }
}