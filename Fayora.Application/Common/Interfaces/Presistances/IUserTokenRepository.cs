using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserTokenRepository
{
    void AddToken(UserTokens refreshToken);
    Task<UserTokens?> GetTokenAsync(Guid UserId, TokenType tokenType, string tokenHash, CancellationToken cancellationToken = default, bool isTracking = false);
    Task RevokeTokensForDeviceAsync(Guid userId, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}