using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class Project : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string CoverImageUrl { get; set; }
        public string Client { get; set; }
        public DateTime? ProjectDate { get; set; }
        public string? ProjectUrl { get; set; }
        public string? GithubUrl { get; set; }
        public int Order { get; set; }

        public ICollection<ProjectCategoryMapping> CategoryMappings { get; set; }
        public ICollection<ProjectImage> ProjectImages { get; set; }
        public ICollection<ProjectFeature> ProjectFeatures { get; set; }
    }
}