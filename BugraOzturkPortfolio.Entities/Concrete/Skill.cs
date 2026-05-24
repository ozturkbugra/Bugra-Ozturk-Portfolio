using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class Skill : BaseEntity
    {
        public string Title { get; set; }
        public int Percentage { get; set; } 
        public string GroupName { get; set; } 
        public int Order { get; set; }
    }
}