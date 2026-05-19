using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class SiteSetting : BaseEntity
    {
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string LogoText { get; set; }
        public string FaviconUrl { get; set; }
        public string AppleTouchIconUrl { get; set; }
        public string FooterText { get; set; }
        public string? GoogleAnalyticsCode { get; set; }
    }
}