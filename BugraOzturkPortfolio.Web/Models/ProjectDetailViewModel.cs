using BugraOzturkPortfolio.Entities.Concrete;
using System.Collections.Generic;

namespace BugraOzturkPortfolio.Web.Models
{
    public class ProjectDetailViewModel
    {
        public Project Project { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProjectFeature> ProjectFeatures { get; set; }
    }
}