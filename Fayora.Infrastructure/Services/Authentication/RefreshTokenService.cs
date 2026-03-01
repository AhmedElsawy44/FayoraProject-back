using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services.Authentication;

public class RefreshTokenService : IRefreshTokenService
{
    public string GenerateTokenString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
