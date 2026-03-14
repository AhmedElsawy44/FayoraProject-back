using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IJwtService
{
    string GenerateToken(string deviceId, User user, Guid? touristId = null, Guid? tourGuideId = null, Guid? ownerId = null);
    public int ExpiresIn { get; }
}
