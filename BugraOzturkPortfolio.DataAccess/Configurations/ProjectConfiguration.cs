using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(150);
            builder.Property(p => p.ShortDescription).IsRequired().HasMaxLength(250);
            builder.Property(p => p.FullDescription).IsRequired();
            builder.Property(p => p.CoverImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Client).HasMaxLength(100);
            builder.Property(p => p.ProjectUrl).HasMaxLength(500);
            builder.Property(p => p.GithubUrl).HasMaxLength(500);

            builder.HasIndex(p => p.Slug).IsUnique();
        }
    }
}