using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IUserTokenRepository
{
    void AddToken(UserTokens refreshToken);
    Task<UserTokens?> GetTokenAsync(Guid UserId, TokenType tokenType, string tokenHash, CancellationToken cancellationToken = default, bool IsReadonly = false);
    Task RevokeTokensForDeviceAsync(Guid userId, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserTokens?> GetTokenByHashAsync( string tokenHash, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default);

}