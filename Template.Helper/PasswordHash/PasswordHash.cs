using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Logging;

namespace Template.Helper.PasswordHash
{
    public class PasswordHash : IPasswordHash
    {
        private readonly ILogger<PasswordHash> _logger;

        public PasswordHash(ILogger<PasswordHash> logger)
        {
            _logger = logger;
        }

        public string Encrypt(string password)
        {
           _logger.LogInformation($"call: Encrypt");

            string passwordHash = Argon2.Hash(password);

            _logger.LogDebug($"data: {passwordHash}");

            return passwordHash;
        }

        public bool Verify(string passwordHash, string password)
        {
            _logger.LogInformation($"call: Verify");

            bool isVerify = Argon2.Verify(passwordHash, password);

            return isVerify;
        }
    }
}