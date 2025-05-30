using Isopoh.Cryptography.Argon2;

namespace Template.Helper
{
    public class PasswordHash
    {
        public static string Encrypt(string password)
        {
            string passwordHash = Argon2.Hash(password);

            return passwordHash;
        }

        public static bool Verify(string passwordHash, string password)
        {
            bool isVerify = Argon2.Verify(passwordHash, password);

            return isVerify;
        }
    }
}