using Microsoft.EntityFrameworkCore;
using BugraOzturkPortfolio.Entities.Concrete;
using System.Reflection;

namespace BugraOzturkPortfolio.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCategoryMapping> ProjectCategoryMappings { get; set; }
        public DbSet<ProjectFeature> ProjectFeatures { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<SiteScript> SiteScripts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}