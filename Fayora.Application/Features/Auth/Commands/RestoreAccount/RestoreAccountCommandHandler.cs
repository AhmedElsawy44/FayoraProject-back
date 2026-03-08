using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccount
{
    public class RestoreAccountCommandHandler(
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository,
        IUserTokenRepository userTokenRepository,
        IDeviceRepository deviceRepository,
        IJwtService jwtService,
        IUserTokenService userTokenService,
        ITokenHasher tokenHasher,
        ICodeHasher codeHasher,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork
    ) : IRequestHandler<RestoreAccountCommand, Result<AuthResult>>
    {
        public async Task<Result<AuthResult>> Handle(
            RestoreAccountCommand request,
            CancellationToken cancellationToken)
        {
            // 1 - Get The User
            var options = new UserQueryOptions
            {
                IsReadOnly = false,
                IncludeVerificationCodes = true,
                IncludeRoles = true,
                UserStatus = UserStatus.Deleted
            };

            User? user = request.IsEmail
                ? await userRepository.GetUserByEmailAsync(request.Identifier, options, cancellationToken)
                : await userRepository.GetUserByPhoneAsync(request.Identifier, options, cancellationToken);

            if (user is null)
                return AuthErrors.UserNotFound;

            // 2 - Check if the user account is actually deleted 
            if (!user.IsDeleted)
                return AuthErrors.UserNotFound;

            // 3 - Check OTP
            var otp = await verificationCodeRepository.GetUserCodeAsync(
                user.Id,
                request.Identifier,
                CodePurpose.ReactivateAccount,
                cancellationToken);

            if (otp is null)
                return AuthErrors.InvalidVerificationCode;

            var verifyResult = otp.Use(request.Code, codeHasher);
            if (verifyResult.IsError)
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return verifyResult.Errors;
            }

            // 4 - Restore Account
            user.Restore();

            // 5 - Add or Update Device
            var device = await deviceRepository
                .GetDeviceByUserIdAndDeviceIdAsync(
                    user.Id,
                    request.DeviceId,
                    cancellationToken,
                    isTracking: true);

            if (device is null)
            {
                deviceRepository.AddDevice(new UserDevice(
                    user.Id,
                    request.DeviceId,
                    request.FcmToken,
                    request.DeviceLanguage));
            }
            else
            {
                device.UpdateInfo(request.FcmToken, request.DeviceLanguage);
            }

            // 6 - Generate Tokens
            var accessToken = jwtService.GenerateToken(request.DeviceId, user);
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

            user.Login();

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new AuthResult(
                user.Id,
                request.Identifier,
                accessToken,
                refreshTokenString);
        }
    }
}
