using Fayora.Domain.Common;
using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
