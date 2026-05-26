using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.Enums.TourGuideModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Infrastructure.Services.AuthModule;

public class AuthTokenGenerator(
    IJwtService jwtService,
    IUserTokenService userTokenService,
    ITokenHasher tokenHasher,
    IUserTokenRepository userTokenRepository,
    ITourGuideRepository tourGuideRepository,
    ITourCompanyRepository tourCompanyRepository,
    IClientContextProvider clientContextProvider) : IAuthTokenGenerator
{
    public async Task<AuthTokensDto> GenerateTokensAsync(
        User user,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        var isAccountVerified = true;
        if (user.Roles.HasValue && user.Roles.Value.HasFlag(Role.TourGuide))
        {
            var tourGuide = await tourGuideRepository.GetGuideByIdAsync(user.Id, new GuideQueryOptions(), cancellationToken);
            if (tourGuide != null)
                isAccountVerified = tourGuide.Status == ItemStatus.Active;
            else isAccountVerified = false;
        }
        if (user.Roles.HasValue && user.Roles.Value.HasFlag(Role.TourCompany))
        {
            var company = await tourCompanyRepository.GetTourCompanyByIdAsync(user.Id, new GuideQueryOptions(), cancellationToken);
            if (company != null)
                isAccountVerified = company.Status == ItemStatus.Active;
            else isAccountVerified = false;
        }
        var accessToken = jwtService.GenerateToken(deviceId, user, isAccountVerified);

        var refreshTokenString = userTokenService.GenerateTokenString();
        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);
        var ipAddress = clientContextProvider.GetContext().IpAddress;

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, deviceId, ipAddress);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, deviceId, TokenType.RefreshToken, cancellationToken);
        userTokenRepository.AddToken(refreshToken);

        return new AuthTokensDto(accessToken, refreshTokenString, jwtService.ExpiresIn);
    }
}