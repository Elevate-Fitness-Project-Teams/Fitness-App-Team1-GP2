using AuthenticationService.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            return _hasher.HashPassword(new object(), password);
        }

        public (bool Verified, bool RehashNeeded, string? NewHash) VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return (false, false, null);

            var result = _hasher.VerifyHashedPassword(new object(), hashedPassword, password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var newHash = HashPassword(password);
                return (true, true, newHash);
            }

            if (result == PasswordVerificationResult.Success)
            {
                return (true, false, null);
            }

            return (false, false, null);
        }
    }
}
