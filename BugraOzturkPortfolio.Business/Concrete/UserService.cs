using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Business.Security;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _unitOfWork.GetRepository<User>().GetAllAsync())
                .Where(u => !u.IsDeleted)
                .ToList();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _unitOfWork.GetRepository<User>().GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> SaveUserAsync(User model, string? plainPassword = null)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                return (false, "Lütfen gerekli tüm alanları doldurunuz!");

            var repo = _unitOfWork.GetRepository<User>();
            var allUsers = await repo.GetAllAsync();

            if (model.Id == Guid.Empty)
            {
                if (string.IsNullOrEmpty(plainPassword))
                    return (false, "Yeni kullanıcı için şifre zorunludur!");

                if (allUsers.Any(u => u.Username.ToLower() == model.Username.ToLower() && !u.IsDeleted))
                    return (false, "Bu kullanıcı adı zaten sistemde kayıtlı!");

                if (allUsers.Any(u => u.Email.ToLower() == model.Email.ToLower() && !u.IsDeleted))
                    return (false, "Bu e-posta adresi zaten sistemde kayıtlı!");

                model.PasswordHash = PasswordHasher.HashPassword(plainPassword);
                model.IsTwoFactorEnabled = false;
                model.IsDeleted = false;
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;

                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Kullanıcı başarıyla eklendi.");
            }
            else
            {
                var existUser = await repo.GetByIdAsync(model.Id);
                if (existUser == null || existUser.IsDeleted)
                    return (false, "Güncellenecek kullanıcı bulunamadı!");

                if (allUsers.Any(u => u.Username.ToLower() == model.Username.ToLower() && u.Id != model.Id && !u.IsDeleted))
                    return (false, "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor!");

                if (allUsers.Any(u => u.Email.ToLower() == model.Email.ToLower() && u.Id != model.Id && !u.IsDeleted))
                    return (false, "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor!");

                existUser.Username = model.Username;
                existUser.Email = model.Email;
                existUser.FirstName = model.FirstName;
                existUser.LastName = model.LastName;
                existUser.UpdatedDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(plainPassword))
                {
                    existUser.PasswordHash = PasswordHasher.HashPassword(plainPassword);
                }

                repo.Update(existUser);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Kullanıcı bilgileri başarıyla güncellendi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var existUser = await repo.GetByIdAsync(id);
            if (existUser == null || existUser.IsDeleted)
                return (false, "Kullanıcı bulunamadı!");

            existUser.IsDeleted = true;
            existUser.UpdatedDate = DateTime.UtcNow;

            repo.Update(existUser);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Kullanıcı başarıyla silindi (arşivlendi).");
        }

        public async Task<(bool Success, string Message)> ResetUserTwoFactorAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var existUser = await repo.GetByIdAsync(id);
            if (existUser == null || existUser.IsDeleted)
                return (false, "Kullanıcı bulunamadı!");

            existUser.IsTwoFactorEnabled = false;
            existUser.TwoFactorSecretKey = null;
            existUser.UpdatedDate = DateTime.UtcNow;

            repo.Update(existUser);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Kullanıcının 2FA koruması başarıyla sıfırlandı.");
        }
    }
}