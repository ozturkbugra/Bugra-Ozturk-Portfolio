using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Business.Concrete;
using BugraOzturkPortfolio.DataAccess.Context;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.DataAccess.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IServiceService, ServiceService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
