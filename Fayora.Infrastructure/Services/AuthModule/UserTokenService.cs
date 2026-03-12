using Fayora.Application.Common.Interfaces.Services.AuthServices;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services.AuthServices;

public class UserTokenService : IUserTokenService
{
    public string GenerateTokenString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
