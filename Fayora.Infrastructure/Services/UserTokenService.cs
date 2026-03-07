using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services;

public class UserTokenService : IUserTokenService
{
    public string GenerateTokenString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
