namespace BugraOzturkPortfolio.Web.Models
{
    public class MailDisplayViewModel
    {
        public string UniqueId { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Snippet { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }

        public List<MailAttachmentViewModel> Attachments { get; set; } = new List<MailAttachmentViewModel>();
    }

    public class MailAttachmentViewModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Base64Data { get; set; } 
    }
}