using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IJwtService
{
    string GenerateToken(string deviceId, User user, bool isAccountVerified = true);
    public int ExpiresIn { get; }
}
