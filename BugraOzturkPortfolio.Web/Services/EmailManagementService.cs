using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using BugraOzturkPortfolio.Web.Models;

namespace BugraOzturkPortfolio.Web.Services
{
    public class EmailManagementService
    {
        private readonly IConfiguration _config;

        public EmailManagementService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<MailDisplayViewModel>> GetInboxMessagesAsync(int count = 20)
        {
            var emails = new List<MailDisplayViewModel>();

            using (var client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_config["EmailSettings:ImapServer"], int.Parse(_config["EmailSettings:ImapPort"]), true);
                await client.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                int startIndex = Math.Max(0, inbox.Count - count);
                for (int i = inbox.Count - 1; i >= startIndex; i--)
                {
                    var message = await inbox.GetMessageAsync(i);
                    var sender = message.From[0] as MailboxAddress;

                    emails.Add(new MailDisplayViewModel
                    {
                        UniqueId = message.MessageId,
                        FromName = sender?.Name ?? "Bilinmeyen Gönderen",
                        FromAddress = sender?.Address ?? "",
                        Subject = message.Subject ?? "(Konu Yok)",
                        Snippet = message.TextBody != null ? (message.TextBody.Length > 100 ? message.TextBody.Substring(0, 100) + "..." : message.TextBody) : "",
                        Body = message.HtmlBody ?? message.TextBody ?? "",
                        Date = message.Date.DateTime.ToLocalTime(),
                        IsRead = true 
                    });
                }

                await client.DisconnectAsync(true);
            }
            return emails;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:EmailAddress"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Sertifika doğrulamalarını paylaşımlı sunucu için esnetiyoruz aga
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // ÇÖZÜM DOKUNUŞU: StartTls yerine Auto veya StartTlsWhenAvailable kullanarak el sıkışmayı sunucuya bırakıyoruz!
                await client.ConnectAsync(
                    _config["EmailSettings:SmtpServer"],
                    int.Parse(_config["EmailSettings:SmtpPort"]),
                    MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable
                );

                await client.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            return true;
        }
    }
}