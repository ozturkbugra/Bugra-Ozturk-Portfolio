using BugraOzturkPortfolio.Business.Abstract;
using BugraOzturkPortfolio.Business.Security;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using OtpNet;

namespace BugraOzturkPortfolio.Business.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = (await repo.GetAllAsync())
                .FirstOrDefault(u => u.Username == username && !u.IsDeleted);

            if (user == null)
                return (false, "Kullanıcı adı veya şifre hatalı!", null);

            var isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            if (!isPasswordValid)
                return (false, "Kullanıcı adı veya şifre hatalı!", null);

            return (true, "Giriş başarılı.", user);
        }

        public async Task<(bool Success, string SecretKey, string QrCodeUrl)> GenerateTwoFactorSecretAsync(Guid userId)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = await repo.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
                return (false, string.Empty, string.Empty);

            if (string.IsNullOrEmpty(user.TwoFactorSecretKey))
            {
                byte[] secretBytes = KeyGeneration.GenerateRandomKey(20);
                user.TwoFactorSecretKey = Base32Encoding.ToString(secretBytes);

                repo.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            string issuer = "BugraPortfolio";
            string qrCodeUrl = $"otpauth://totp/{issuer}:{user.Username}?secret={user.TwoFactorSecretKey}&issuer={issuer}&digits=6&period=30";

            return (true, user.TwoFactorSecretKey, qrCodeUrl);
        }

        public async Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = await repo.GetByIdAsync(userId);

            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecretKey) || user.IsDeleted)
                return false;

            try
            {
                byte[] secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecretKey);
                var totp = new Totp(secretBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);

                long timeStepMatched = 0;
                bool isValid = totp.VerifyTotp(code, out timeStepMatched, new VerificationWindow(previous: 1, future: 1));

                return isValid;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success, string Message)> ToggleTwoFactorAsync(Guid userId, bool enable)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = await repo.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
                return (false, "Kullanıcı bulunamadı.");

            if (enable && string.IsNullOrEmpty(user.TwoFactorSecretKey))
                return (false, "Önce bir 2FA anahtarı üretilmelidir.");

            user.IsTwoFactorEnabled = enable;
            user.UpdatedDate = DateTime.UtcNow;

            if (!enable)
            {
                user.TwoFactorSecretKey = null;
            }

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, enable ? "2FA başarıyla aktifleştirildi." : "2FA devre dışı bırakıldı.");
        }

        public async Task<(bool Success, string Message, string? Token)> GeneratePasswordResetTokenAsync(string email)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = (await repo.GetAllAsync())
                .FirstOrDefault(u => u.Email == email && !u.IsDeleted);

            if (user == null)
                return (false, "Bu e-posta adresine ait kullanıcı bulunamadı.", null);

            string token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            user.PasswordResetToken = token;
            user.ResetTokenExpireDate = DateTime.UtcNow.AddHours(2);
            user.UpdatedDate = DateTime.UtcNow;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Şifre sıfırlama token'ı başarıyla üretildi.", token);
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var user = (await repo.GetAllAsync())
                .FirstOrDefault(u => u.Email == email && u.PasswordResetToken == token && !u.IsDeleted);

            if (user == null)
                return (false, "Geçersiz şifre sıfırlama talebi veya token.");

            if (user.ResetTokenExpireDate < DateTime.UtcNow)
                return (false, "Şifre sıfırlama linkinin süresi dolmuş.");

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpireDate = null;
            user.UpdatedDate = DateTime.UtcNow;

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.");
        }
    }
}