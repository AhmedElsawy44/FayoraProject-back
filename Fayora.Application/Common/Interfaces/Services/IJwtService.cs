using Fayora.Domain.Common;
using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(string deviceId, User user, IEnumerable<string>? roles = null);
}
