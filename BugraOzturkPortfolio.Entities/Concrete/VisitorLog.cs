using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class VisitorLog : BaseEntity
    {
        public string VisitorHash { get; set; }
        public DateTime VisitDate { get; set; }
    }
}