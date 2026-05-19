namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class ProjectCategoryMapping
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}