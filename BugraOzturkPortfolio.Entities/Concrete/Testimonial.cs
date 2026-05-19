using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class Testimonial : BaseEntity
    {
        public string FullName { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Comment { get; set; }
        public string? ImageUrl { get; set; }
        public int Order { get; set; }
    }
}