using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class ProjectImage : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
    }
}