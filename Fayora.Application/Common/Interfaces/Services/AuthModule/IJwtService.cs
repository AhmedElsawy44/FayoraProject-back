using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IJwtService
{
    string GenerateToken(string deviceId, User user);
    public int ExpiresIn { get; }
}
