namespace BugraOzturkPortfolio.Business.Abstract
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
    }
}