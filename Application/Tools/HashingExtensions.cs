using System.Security.Cryptography;

namespace Application.Tools
{
    public static class HashingExtensions
    {
        public static string HashPassword(this string password, out string salt)
        {
            // Generate a 128-bit salt using a secure PRNG
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16); // 16 bytes = 128 bits
            salt = Convert.ToBase64String(saltBytes);

            // Derive a 256-bit subkey (32 bytes)
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(this string password, string salt, string hashToCompare)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            string computedHash = Convert.ToBase64String(hash);
            return computedHash == hashToCompare;
        }
    }
}
