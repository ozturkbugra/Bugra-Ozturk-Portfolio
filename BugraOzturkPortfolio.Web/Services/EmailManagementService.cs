using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
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

        public async Task<List<MailDisplayViewModel>> GetMessagesFromFolderAsync(string folderName = "INBOX", int count = 20)
        {
            var emails = new List<MailDisplayViewModel>();

            using (var client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_config["EmailSettings:ImapServer"], int.Parse(_config["EmailSettings:ImapPort"]), SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);

                IMailFolder targetFolder = null;

                if (folderName.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                {
                    targetFolder = client.GetFolder("Sent Items")
                                   ?? client.GetFolder(SpecialFolder.Sent)
                                   ?? client.GetFolder("INBOX.Sent")
                                   ?? client.GetFolder("Sent")
                                   ?? client.GetFolder("Sent Messages");
                }
                else if (folderName.Equals("Trash", StringComparison.OrdinalIgnoreCase))
                {
                    targetFolder = client.GetFolder("INBOX.Trash")
                                   ?? client.GetFolder(SpecialFolder.Trash)
                                   ?? client.GetFolder("Trash");
                }

                if (targetFolder == null)
                {
                    targetFolder = client.Inbox;
                }

                await targetFolder.OpenAsync(FolderAccess.ReadOnly);

                int startIndex = Math.Max(0, targetFolder.Count - count);
                for (int i = targetFolder.Count - 1; i >= startIndex; i--)
                {
                    var message = await targetFolder.GetMessageAsync(i);

                    var sender = message.From.Count > 0 ? message.From[0] as MailboxAddress : null;
                    var recipient = message.To.Count > 0 ? message.To[0] as MailboxAddress : null;

                    DateTime trMailDate = message.Date.LocalDateTime;

                    var attachmentList = new List<MailAttachmentViewModel>();
                    string mailBodyHtml = message.HtmlBody ?? message.TextBody ?? "";

                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment is MimePart mimePart)
                        {
                            using (var memory = new MemoryStream())
                            {
                                await mimePart.Content.DecodeToAsync(memory);
                                var byteData = memory.ToArray();
                                string base64String = Convert.ToBase64String(byteData);

                                if (!string.IsNullOrEmpty(mimePart.ContentId) && mimePart.ContentLocation == null)
                                {
                                    string srcUri = $"data:{mimePart.ContentType.MimeType};base64,{base64String}";
                                    mailBodyHtml = mailBodyHtml.Replace($"cid:{mimePart.ContentId}", srcUri);
                                }
                                else 
                                {
                                    attachmentList.Add(new MailAttachmentViewModel
                                    {
                                        FileName = mimePart.FileName ?? "unnamed_file",
                                        ContentType = mimePart.ContentType.MimeType,
                                        Base64Data = base64String
                                    });
                                }
                            }
                        }
                    }

                    emails.Add(new MailDisplayViewModel
                    {
                        UniqueId = message.MessageId ?? Guid.NewGuid().ToString(),
                        FromName = folderName.Equals("Sent", StringComparison.OrdinalIgnoreCase)
                            ? ($"Kime: {recipient?.Name ?? recipient?.Address}")
                            : (sender?.Name ?? "Bilinmeyen Gönderen"),
                        FromAddress = sender?.Address ?? "",
                        Subject = message.Subject ?? "(Konu Yok)",
                        Snippet = message.TextBody != null ? (message.TextBody.Length > 100 ? message.TextBody.Substring(0, 100) + "..." : message.TextBody) : "",
                        Body = mailBodyHtml,
                        Date = trMailDate,
                        IsRead = true,
                        Attachments = attachmentList 
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

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtpClient.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTlsWhenAvailable);
                await smtpClient.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }

            try
            {
                using (var imapClient = new ImapClient())
                {
                    imapClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await imapClient.ConnectAsync(_config["EmailSettings:ImapServer"], int.Parse(_config["EmailSettings:ImapPort"]), SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(_config["EmailSettings:EmailAddress"], _config["EmailSettings:Password"]);

                    var sentFolder = imapClient.GetFolder("Sent Items")
                                     ?? imapClient.GetFolder("INBOX.Sent")
                                     ?? imapClient.GetFolder(SpecialFolder.Sent)
                                     ?? imapClient.GetFolder("Sent");
                    if (sentFolder != null)
                    {
                        await sentFolder.OpenAsync(FolderAccess.ReadWrite);
                        await sentFolder.AppendAsync(emailMessage, MessageFlags.Seen);
                    }
                    await imapClient.DisconnectAsync(true);
                }
            }
            catch { }

            return true;
        }
    }
}