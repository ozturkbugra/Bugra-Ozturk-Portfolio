using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.InstitutionName).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Degree).IsRequired().HasMaxLength(100);
            builder.Property(e => e.FieldOfStudy).IsRequired().HasMaxLength(100);
            builder.Property(e => e.StartDate).IsRequired();
        }
    }
}