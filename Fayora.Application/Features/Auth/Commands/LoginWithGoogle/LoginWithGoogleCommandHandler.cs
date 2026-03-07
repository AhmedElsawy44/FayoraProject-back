using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.ValueObjects;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.LoginWithGoogle;

public class LoginWithGoogleCommandHandler(
    IUserIdentityRepository userIdentityRepository,
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IGoogleAuthService googleAuthService,
    IJwtService jwtService,
    ITokenHasher tokenHasher,
    IUserTokenService userTokenService,
    IClientContextProvider clientContextProvider)
    : IRequestHandler<LoginWithGoogleCommand, Result<LoginWithGoogleResult>>
{
    public async Task<Result<LoginWithGoogleResult>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await googleAuthService.GetUserInfoAsync(request.IdToken, cancellationToken);
        if (googleUser is null) return AuthErrors.InvalidCredentials;

        var options = new UserQueryOptions { IsReadOnly = false, IncludeRoles = true };
        User? user;

        var identity = await userIdentityRepository.GetIdentityByIdAsync(googleUser.Id, IdentityProvider.Google, cancellationToken);

        if (identity is not null)
        {
            user = await userRepository.GetUserByIdAsync(identity.UserId, options, cancellationToken);
            if (user is null) return AuthErrors.UserNotFound;
        }
        else
        {
            user = await userRepository.GetUserByEmailAsync(googleUser.Email, options, cancellationToken);

            if (user is null)
            {
                user = User.CreateWithSocialLogin(googleUser.Email, googleUser.FirstName, googleUser.LastName, googleUser.PictureUrl);

                userRepository.AddUser(user);
            }

            var email = Email.Create(googleUser.Email);

            var newIdentity = new UserIdentity(user.Id, IdentityProvider.Google, googleUser.Id, email.Value);
            userIdentityRepository.AddIdentity(newIdentity);
        }

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        user.Login();

        var device = await deviceRepository.GetDeviceByUserIdAndDeviceIdAsync(user.Id, request.DeviceId, cancellationToken, isTracking: true);

        if (device is null)
        {
            device = new UserDevice(user.Id, request.DeviceId, request.FcmToken, request.DeviceLanguage);
            deviceRepository.AddDevice(device);
        }
        else
        {
            device.UpdateInfo(request.FcmToken, request.DeviceLanguage);
        }

        var accessToken = jwtService.GenerateToken(request.DeviceId, user);
        var refreshTokenString = userTokenService.GenerateTokenString();
        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, request.DeviceId, clientContextProvider.GetContext().IpAddress);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, request.DeviceId, TokenType.RefreshToken, cancellationToken);
        userTokenRepository.AddToken(refreshToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new LoginWithGoogleResult(user.Id, googleUser.Email, accessToken, refreshTokenString, jwtService.ExpiresIn);
    }
}