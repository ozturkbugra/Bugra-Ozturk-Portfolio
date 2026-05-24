using System.Security.Cryptography;

namespace BugraOzturkPortfolio.Business.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32;  // 256-bit
        private const int Iterations = 100000; 
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

        public static string HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithm))
            {
                var key = algorithm.GetBytes(KeySize);
                var salt = algorithm.Salt;

                var hashBytes = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(key, 0, hashBytes, SaltSize, KeySize);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, key, 0, KeySize);

            using (var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
            {
                var verifiedKey = algorithm.GetBytes(KeySize);
                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[i + SaltSize] != verifiedKey[i])
                        return false;
                }
                return true;
            }
        }
    }
}