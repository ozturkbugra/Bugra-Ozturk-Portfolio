using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.FullName).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Company).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Position).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Comment).IsRequired().HasMaxLength(500);
            builder.Property(t => t.ImageUrl).HasMaxLength(500);
        }
    }
}