using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
    {
        public void Configure(EntityTypeBuilder<SiteSetting> builder)
        {
            builder.HasKey(ss => ss.Id);
            builder.Property(ss => ss.MetaTitle).IsRequired().HasMaxLength(70);
            builder.Property(ss => ss.MetaKeywords).IsRequired().HasMaxLength(200);
            builder.Property(ss => ss.MetaDescription).IsRequired().HasMaxLength(160);
            builder.Property(ss => ss.LogoText).IsRequired().HasMaxLength(50);
            builder.Property(ss => ss.FaviconUrl).IsRequired().HasMaxLength(500);
            builder.Property(ss => ss.AppleTouchIconUrl).IsRequired().HasMaxLength(500);
            builder.Property(ss => ss.FooterText).IsRequired().HasMaxLength(150);
            builder.Property(ss => ss.GoogleAnalyticsCode).HasMaxLength(50);
        }
    }
}