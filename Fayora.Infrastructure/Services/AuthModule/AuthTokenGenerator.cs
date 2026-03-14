using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Infrastructure.Services.AuthModule;

public class AuthTokenGenerator(
    IJwtService jwtService,
    IUserTokenService userTokenService,
    ITokenHasher tokenHasher,
    IUserTokenRepository userTokenRepository,
    IClientContextProvider clientContextProvider) : IAuthTokenGenerator
{
    public async Task<AuthTokensDto> GenerateTokensAsync(
        User user,
        string deviceId,
        Guid? touristId = null,
        Guid? tourGuideId = null,
        Guid? ownerId = null,
        CancellationToken cancellationToken = default)
    {
        var accessToken = jwtService.GenerateToken(deviceId, user, touristId, tourGuideId, ownerId);

        var refreshTokenString = userTokenService.GenerateTokenString();
        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);
        var ipAddress = clientContextProvider.GetContext().IpAddress;

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, deviceId, ipAddress);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, deviceId, TokenType.RefreshToken, cancellationToken);
        userTokenRepository.AddToken(refreshToken);

        return new AuthTokensDto(accessToken, refreshTokenString, jwtService.ExpiresIn);
    }
}