using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.GroupName)
                .IsRequired()
                .HasMaxLength(500);

        
            builder.Property(s => s.Percentage)
                .IsRequired();

            builder.Property(s => s.Order)
                .IsRequired();

            builder.Property(s => s.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}