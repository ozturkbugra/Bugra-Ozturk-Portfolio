using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class ContactMessage : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}