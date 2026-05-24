using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class SiteScript : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Code { get; set; }
        public ScriptPosition Position { get; set; }
        public bool IsActive { get; set; } = true;
    }
}