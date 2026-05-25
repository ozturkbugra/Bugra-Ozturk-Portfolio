using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).IsRequired();
            builder.Property(s => s.IconClass).IsRequired().HasMaxLength(50);
        }
    }
}