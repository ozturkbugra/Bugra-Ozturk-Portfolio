using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.DataAccess.Configurations
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.HasKey(cm => cm.Id);
            builder.Property(cm => cm.FullName).IsRequired().HasMaxLength(100);
            builder.Property(cm => cm.Email).IsRequired().HasMaxLength(100);
            builder.Property(cm => cm.Subject).IsRequired().HasMaxLength(150);
            builder.Property(cm => cm.Body).IsRequired().HasMaxLength(2000);
            builder.Property(cm => cm.IpAddress).IsRequired().HasMaxLength(45);
        }
    }
}