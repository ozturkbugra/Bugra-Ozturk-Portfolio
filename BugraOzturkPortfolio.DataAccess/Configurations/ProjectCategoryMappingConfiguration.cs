using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ProjectCategoryMappingConfiguration : IEntityTypeConfiguration<ProjectCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<ProjectCategoryMapping> builder)
        {
            builder.HasKey(pcm => new { pcm.ProjectId, pcm.CategoryId });

            builder.HasOne(pcm => pcm.Project)
                .WithMany(p => p.CategoryMappings)
                .HasForeignKey(pcm => pcm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pcm => pcm.Category)
                .WithMany(c => c.ProjectMappings)
                .HasForeignKey(pcm => pcm.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}