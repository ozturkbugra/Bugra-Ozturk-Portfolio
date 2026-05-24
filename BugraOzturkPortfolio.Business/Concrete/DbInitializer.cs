using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using BugraOzturkPortfolio.Business.Security;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IUnitOfWork unitOfWork)
        {
            var userRepo = unitOfWork.GetRepository<User>();

            var users = await userRepo.GetAllAsync();
            if (!users.Any(u => !u.IsDeleted))
            {
                var defaultAdmin = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "info@bugraozturk.com.tr", 
                    FirstName = "Buğra",
                    LastName = "Öztürk",
                    PasswordHash = PasswordHasher.HashPassword("123456"),
                    IsTwoFactorEnabled = false,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                };

                await userRepo.AddAsync(defaultAdmin);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}