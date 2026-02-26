using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using System.Text.RegularExpressions;

namespace Fayora.Infrastructure.Services.Authentication
{
    public partial class PasswordHasher : IPasswordHasher
    {
        private static readonly Regex PasswordRegex = StrongPasswordRegex();
        public Result<string> Hash(string password)
        {
            return PasswordRegex.IsMatch(password) 
                ? BCrypt.Net.BCrypt.EnhancedHashPassword(password)
                : Error.Validation("Password too weak");
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled)]
        private static partial Regex StrongPasswordRegex();
    }
}
