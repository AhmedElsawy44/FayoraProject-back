using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Infrastructure.Services.AuthServices;

public class AuthTokenGenerator(
    IJwtService jwtService,
    IUserTokenService userTokenService,
    ITokenHasher tokenHasher,
    IUserTokenRepository userTokenRepository,
    IClientContextProvider clientContextProvider) : IAuthTokenGenerator
{
    public async Task<AuthTokensDto> GenerateTokensAsync(User user, string deviceId, CancellationToken cancellationToken = default)
    {
        var accessToken = jwtService.GenerateToken(deviceId, user);

        var refreshTokenString = userTokenService.GenerateTokenString();
        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);
        var ipAddress = clientContextProvider.GetContext().IpAddress;

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, deviceId, ipAddress);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, deviceId, TokenType.RefreshToken, cancellationToken);
        userTokenRepository.AddToken(refreshToken);

        return new AuthTokensDto(accessToken, refreshTokenString, jwtService.ExpiresIn);
    }
}