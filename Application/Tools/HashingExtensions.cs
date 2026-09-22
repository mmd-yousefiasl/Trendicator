using System.Security.Cryptography;

namespace Application.Tools
{
    public static class HashingExtensions
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static string HashPassword(this string password, out string salt)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            salt = Convert.ToBase64String(saltBytes);

            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(this string password, string salt, string hashToCompare)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] originalHashBytes = Convert.FromBase64String(hashToCompare);

            byte[] computedHashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return CryptographicOperations.FixedTimeEquals(computedHashBytes, originalHashBytes);
        }
    }
}
