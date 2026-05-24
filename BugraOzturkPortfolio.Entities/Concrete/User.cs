using BugraOzturkPortfolio.Entities.Common;

namespace BugraOzturkPortfolio.Entities.Concrete
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Two-Factor Authentication (2FA) Alanları
        public bool IsTwoFactorEnabled { get; set; }
        public string? TwoFactorSecretKey { get; set; } // Google Authenticator için gizli anahtar

        // Şifre Sıfırlama Alanları
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpireDate { get; set; }
    }
}