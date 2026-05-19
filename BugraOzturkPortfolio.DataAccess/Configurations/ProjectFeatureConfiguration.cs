using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ProjectFeatureConfiguration : IEntityTypeConfiguration<ProjectFeature>
    {
        public void Configure(EntityTypeBuilder<ProjectFeature> builder)
        {
            builder.HasKey(pf => pf.Id);
            builder.Property(pf => pf.Title).IsRequired().HasMaxLength(100);
            builder.Property(pf => pf.Description).IsRequired().HasMaxLength(250);
            builder.Property(pf => pf.IconClass).IsRequired().HasMaxLength(50);

            builder.HasOne(pf => pf.Project)
                .WithMany(p => p.ProjectFeatures)
                .HasForeignKey(pf => pf.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}