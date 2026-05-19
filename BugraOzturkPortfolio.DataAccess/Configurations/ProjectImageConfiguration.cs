using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
    {
        public void Configure(EntityTypeBuilder<ProjectImage> builder)
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(pi => pi.Project)
                .WithMany(p => p.ProjectImages)
                .HasForeignKey(pi => pi.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}