using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // Esnek Mail Çekme Metodu: Klasör adını dışarıdan alır aga (Default: INBOX)
        public async Task<List<MailDisplayViewModel>> GetMessagesFromFolderAsync(string folderName = "INBOX", int count = 20)
        {
            var emails = new List<MailDisplayViewModel>();

            using (var client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_config["EmailSettings:ImapServer"], int.Parse(_config["EmailSettings:ImapPort"]), SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);

                // Güzel Hosting üzerindeki ilgili klasörü buluyoruz aga (Sent, Trash, INBOX vb.)
                // EmailManagementService.cs içindeki ilgili alanı değiştiriyoruz aga:
                IMailFolder targetFolder = null;

                if (folderName.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                {
                    // Güzel Hosting / cPanel'in tüm sinsi varyasyonlarını sırayla deniyoruz aga
                    targetFolder = client.GetFolder(SpecialFolder.Sent)
                                   ?? client.GetFolder("INBOX.Sent")
                                   ?? client.GetFolder("Sent")
                                   ?? client.GetFolder("Sent Messages")
                                   ?? client.GetFolder("Sent Items");
                }
                else if (folderName.Equals("Trash", StringComparison.OrdinalIgnoreCase))
                {
                    targetFolder = client.GetFolder(SpecialFolder.Trash)
                                   ?? client.GetFolder("INBOX.Trash")
                                   ?? client.GetFolder("Trash");
                }

                // Eğer yukarıdakiler yine de null dönerse veya klasör INBOX ise varsayılana çek
                if (targetFolder == null)
                {
                    targetFolder = client.Inbox;
                }

                await targetFolder.OpenAsync(FolderAccess.ReadOnly);

                int startIndex = Math.Max(0, targetFolder.Count - count);
                for (int i = targetFolder.Count - 1; i >= startIndex; i--)
                {
                    var message = await targetFolder.GetMessageAsync(i);

                    // Gönderen veya alıcı bilgisini jilet gibi ayıklıyoruz
                    var sender = message.From.Count > 0 ? message.From[0] as MailboxAddress : null;
                    var recipient = message.To.Count > 0 ? message.To[0] as MailboxAddress : null;

                    emails.Add(new MailDisplayViewModel
                    {
                        UniqueId = message.MessageId ?? Guid.NewGuid().ToString(),
                        FromName = folderName.Equals("Sent", StringComparison.OrdinalIgnoreCase) ? ($"Kime: {recipient?.Name ?? recipient?.Address}") : (sender?.Name ?? "Bilinmeyen Gönderen"),
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

        // SMTP ile Mail Gönderip Ardından Sunucunun 'Sent' Klasörüne Kopyalayan Canavar Metot
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:EmailAddress"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // 1. ADIM: Maili Alıcıya Fırlatıyoruz aga
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtpClient.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTlsWhenAvailable);
                await smtpClient.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }

            // 2. ADIM: Gönderilen mail kaybolmasın diye IMAP ile 'Sent' klasörüne bir kopyasını mühürlüyoruz aga!
            try
            {
                using (var imapClient = new ImapClient())
                {
                    imapClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await imapClient.ConnectAsync(_config["EmailSettings:ImapServer"], int.Parse(_config["EmailSettings:ImapPort"]), SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);

                    var sentFolder = imapClient.GetFolder(SpecialFolder.Sent) ?? imapClient.GetFolder("Sent");
                    if (sentFolder != null)
                    {
                        await sentFolder.OpenAsync(FolderAccess.ReadWrite);
                        await sentFolder.AppendAsync(emailMessage, MessageFlags.Seen);
                    }
                    await imapClient.DisconnectAsync(true);
                }
            }
            catch { /* Gönderme başarılı olduysa kopyalama hatası ana akışı bozmasın aga */ }

            return true;
        }
    }
}