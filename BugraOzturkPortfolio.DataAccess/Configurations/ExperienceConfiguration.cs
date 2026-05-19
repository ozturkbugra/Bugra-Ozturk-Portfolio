using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Position).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            builder.Property(e => e.Location).IsRequired().HasMaxLength(100);
            builder.Property(e => e.StartDate).IsRequired();
        }
    }
}