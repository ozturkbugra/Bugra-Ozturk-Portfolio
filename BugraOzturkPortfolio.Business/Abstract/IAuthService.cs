using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password);
        Task<(bool Success, string SecretKey, string QrCodeUrl)> GenerateTwoFactorSecretAsync(Guid userId);
        Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
        Task<(bool Success, string Message)> ToggleTwoFactorAsync(Guid userId, bool enable);
        Task<(bool Success, string Message, string? Token)> GeneratePasswordResetTokenAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword);
    }
}