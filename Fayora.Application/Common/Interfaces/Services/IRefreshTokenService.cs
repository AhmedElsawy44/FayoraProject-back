using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IRefreshTokenService
{
    RefreshToken GenerateToken();
}
