using Fayora.Domain.Entitties.Identity;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(string deviceId, User user, IEnumerable<string>? roles = null);
}
