using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class AboutConfiguration : IEntityTypeConfiguration<About>
    {
        public void Configure(EntityTypeBuilder<About> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(a => a.CvUrl).HasMaxLength(500);
            builder.Property(a => a.Email).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Phone).IsRequired().HasMaxLength(20);
            builder.Property(a => a.Address).IsRequired().HasMaxLength(250);
            builder.Property(a => a.GithubUrl).HasMaxLength(200);
            builder.Property(a => a.TwitterUrl).HasMaxLength(200);
            builder.Property(a => a.FacebookUrl).HasMaxLength(200);
            builder.Property(a => a.InstagramUrl).HasMaxLength(200);
            builder.Property(a => a.LinkedinUrl).HasMaxLength(200);
        }
    }
}