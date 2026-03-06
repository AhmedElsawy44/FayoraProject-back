using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace Fayora.Infrastructure.Services.Authentication;

public class TokenHasher : ITokenHasher
{
    public string HashToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        byte[] bytes = Encoding.UTF8.GetBytes(token);

        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }
}