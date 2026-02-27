using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IRefreshTokensRepository
{
    void AddToken(RefreshToken refreshToken);
    Task<RefreshToken?> GetTokenAsync(string Token, CancellationToken cancellationToken, bool isTracking = false);
}