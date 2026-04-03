using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IAuthTokenGenerator authTokenGenerator,
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResult>>
{
    public async Task<Result<RefreshTokenResult>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // 1 - Hash The Token that came from frontend
        var hashedToken = tokenHasher.HashToken(request.RefreshToken);

        // 2 - Get the token from DB and check if it's valid 
        var refreshToken = await userTokenRepository.GetTokenByHashAsync(
            hashedToken,
            request.DeviceId,
            TokenType.RefreshToken,
            cancellationToken);

        if (refreshToken is null || !refreshToken.IsValid)
            return AuthErrors.InvalidRefreshToken;

        // 3 - Get The User
        var user = await userRepository.GetUserByIdAsync(
            refreshToken.UserId,
            new UserQueryOptions { IsReadOnly = true, IncludeRoles = true },
            cancellationToken);

        if (user is null)
        {
            await userTokenRepository.RevokeTokensForDeviceAsync(refreshToken.UserId, request.DeviceId, TokenType.RefreshToken, cancellationToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return AuthErrors.InvalidRefreshToken;
        }

        // 4 - Check user Status
        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        // 5 - Cancel the old Refresh Token
        refreshToken.Revoke();

        // 6 - Generate new Access Token and Refresh Token
        Guid? ownerId = null;
        Guid? touristId = null;
        Guid? tourGuideId = null;

        var roleNames = user.GetRoleNames();

        //if (roleNames.Contains("Owner", StringComparer.OrdinalIgnoreCase))
        //{
        //    var owner = await unitOwnerRepository.GetOwnerByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
        //    ownerId = owner?.Id;
        //}

        //if (roleNames.Contains("Tourist", StringComparer.OrdinalIgnoreCase))
        //{
        //    var tourist = await touristRepository.GetProfileByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
        //    touristId = tourist?.Id;
        //}

        //if (roleNames.Contains("TourGuide", StringComparer.OrdinalIgnoreCase))
        //{
        //    var tourGuide = await tourGuideRepository.GetProfileByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
        //    tourGuideId = tourGuide?.Id;
        //}

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            touristId: touristId,
            tourGuideId: tourGuideId,
            ownerId: ownerId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RefreshTokenResult(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}
