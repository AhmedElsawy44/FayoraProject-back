using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using Fayora.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Commands.VerifyRegisterOtp;

public class VerifyRegisterOtpCommandHandler(
    ICodeHasher codeHasher,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IClientContextProvider context,
    IVerificationCodeRepository verificationCodeRepository,
    IUserRepository userRepository,
    IDeviceRepository deviceRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ILogger<VerifyRegisterOtpCommandHandler> logger)
    : IRequestHandler<VerifyRegisterOtpCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(VerifyRegisterOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var identifier = request.Email ?? request.PhoneNumber ?? "";

            var code = await verificationCodeRepository.GetUserCode(request.UserId, identifier, OtpPurpose.Registration, cancellationToken, isTracking: true);

            if (code is null)
                return UserErrors.InvalidOrExpiredOtp;


            var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken, isTracking: true);

            if (user is null)
                return UserErrors.InvalidOrExpiredOtp;

            if(code.IsEmailType && user.IsEmailVerified)
                return AuthErrors.UserAccountIsAlreadyVerified;
            else if (code.IsSmsType && user.IsEmailVerified)
                return AuthErrors.UserAccountIsAlreadyVerified;

            var result = code.Use(request.Code, codeHasher);

            if (result.IsError)
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return result.Errors;
            }

            if (code.IsEmailType)
                user.VerifyEmail();
            else
                user.VerifyPhone();

            var existingDevice = await deviceRepository.GetDeviceByIdAsync(request.DeviceId, cancellationToken, isTracking: true);
            if (existingDevice is null)
            {
                var userDevice = new UserDevice(request.UserId, request.DeviceId, user.PreferredLanguage, request.FcmToken);
                deviceRepository.AddDevice(userDevice);
            }
            else
            {
                existingDevice.UpdateFcmToken(request.FcmToken);
            }

            var accessToken = jwtService.GenerateToken(request.DeviceId, user);
            var refreshTokenString = refreshTokenService.GenerateTokenString();
            var refreshToken = new RefreshToken(user.Id, refreshTokenString, request.DeviceId, context.GetContext().IpAddress);

            refreshTokenRepository.AddToken(refreshToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new AuthResult(user.Id, identifier, accessToken, refreshTokenString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while verifying OTP for User {UserId}", request.UserId);
            return Error.Failure("Server.Error", "An unexpected error occurred during OTP verification.");
        }
    }
}