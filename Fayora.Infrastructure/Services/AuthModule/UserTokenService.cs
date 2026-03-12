using Fayora.Application.Common.Interfaces.Services.AuthModule;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services.AuthModule;

public class UserTokenService : IUserTokenService
{
    public string GenerateTokenString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
