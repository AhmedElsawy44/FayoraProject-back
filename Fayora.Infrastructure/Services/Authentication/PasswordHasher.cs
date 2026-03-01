using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using System.Text.RegularExpressions;

namespace Fayora.Infrastructure.Services.Authentication
{
    public partial class HashingService : IPasswordHasher, ICodeHasher
    {
        private static readonly Regex PasswordRegex = StrongPasswordRegex();
        public Result<string> HashPassword(string password)
        {
            return PasswordRegex.IsMatch(password)
                ? BCrypt.Net.BCrypt.EnhancedHashPassword(password)
                : Error.Validation("Password too weak");
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }
        public string HashCode(string code)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(code);
        }

        public bool VerifyCode(string code, string codeHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(code, codeHash);
        }

        [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled)]
        private static partial Regex StrongPasswordRegex();

    }
}
