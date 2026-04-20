using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Persistences.IdentityModule;

public interface IUserTokenRepository
{
    void AddToken(UserTokens refreshToken);
    Task<UserTokens?> GetTokenAsync(Guid UserId, TokenType tokenType, string tokenHash, CancellationToken cancellationToken = default, bool IsReadonly = false);
    Task RevokeTokensForDeviceAsync(Guid userId, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserTokens?> GetTokenByHashAsync(string tokenHash, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default);

}