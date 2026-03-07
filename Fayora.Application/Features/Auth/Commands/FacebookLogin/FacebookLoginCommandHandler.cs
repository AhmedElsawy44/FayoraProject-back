using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.FacebookLogin
{

    public class FacebookLoginCommandHandler(
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IDeviceRepository deviceRepository,
    IFacebookAuthService facebookAuthService,
    IJwtService jwtService,
    IUserTokenService userTokenService,
    ITokenHasher tokenHasher,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork
) : IRequestHandler<FacebookLoginCommand, Result<AuthResult>>
    {
        public async Task<Result<AuthResult>> Handle(
            FacebookLoginCommand request,
            CancellationToken cancellationToken)
        {
            // 1 - Check Facebook Token
            var facebookUser = await facebookAuthService
                .GetUserInfoAsync(request.AccessToken, cancellationToken);

            if (facebookUser is null)
                return AuthErrors.InvalidCredentials;

            // 2 - Check if user exists in DB by Email
            User? user = null;

            if (!string.IsNullOrWhiteSpace(facebookUser.Email))
                user = await userRepository.GetUserByIdentityAsync(
                    facebookUser.Email,
                    new UserQueryOptions { IsTracking = true, IncludeRoles = true },
                    cancellationToken);

            // 3 - if not, create new user (Register)
            if (user is null)
            {
                user = User.CreateWithSocialLogin(
                    facebookUser.Email,
                    facebookUser.Name,
                    facebookUser.PictureUrl);

                userRepository.AddUser(user);
            }
            else
            {
                // 4 - Check Status
                var statusCheck = user.CheckActiveStatus();
                if (statusCheck.IsError) return statusCheck.Errors;
            }

            // 5 - Login
            user.Login();

            // 6 - Add or Update Device
            var device = await deviceRepository
                .GetDeviceByUserIdAndDeviceIdAsync(
                    user.Id,
                    request.DeviceId,
                    cancellationToken,
                    isTracking: true);

            if (device is null)
            {
                device = new UserDevice(
                    user.Id,
                    request.DeviceId,
                    request.FcmToken,
                    request.DeviceLanguage);
                deviceRepository.AddDevice(device);
            }
            else
            {
                device.UpdateInfo(request.FcmToken, request.DeviceLanguage);
            }

            // 7 - Generate Tokens
            var roles = user.GetRoleNames();
            var accessToken = jwtService.GenerateToken(request.DeviceId, user, roles);
            var refreshTokenString = userTokenService.GenerateTokenString();
            var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);
            var refreshToken = UserTokens.RefreshToken(
                user.Id,
                hashedRefreshToken,
                request.DeviceId,
                clientContextProvider.GetContext().IpAddress);

            await userTokenRepository.RevokeTokensForDeviceAsync(
                user.Id,
                request.DeviceId,
                TokenType.RefreshToken,
                cancellationToken);

            userTokenRepository.AddToken(refreshToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            var identifier = facebookUser.Email ?? facebookUser.Name ?? user.Id.ToString();

            return new AuthResult(
                user.Id,
                identifier,
                accessToken,
                refreshTokenString
            );
        }
    }

}
