using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var smtpHost = _configuration["EmailSettings:Host"];
            var smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var smtpUser = _configuration["EmailSettings:User"];
            var smtpPass = _configuration["EmailSettings:Password"];
            var fromAddress = _configuration["EmailSettings:FromAddress"] ?? smtpUser;

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
                return;

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress!, "BugraPortfolio Yönetim"),
                    Subject = "Şifre Sıfırlama Talebi",
                    Body = $@"<h3>Şifre Sıfırlama Talebi</h3>
                             <p>Hesabınızın şifresini sıfırlamak için aşağıdaki bağlantıya tıklayınız. Bu bağlantı 2 saat geçerlidir.</p>
                             <a href='{resetLink}' style='display:inline-block; background:#0d6efd; color:#fff; padding:10px 20px; text-decoration:none; border-radius:5px;'>Şifremi Sıfırla</a>
                             <p>Eğer bu talebi siz yapmadıysanız, lütfen bu e-postayı dikkate almayınız.</p>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}