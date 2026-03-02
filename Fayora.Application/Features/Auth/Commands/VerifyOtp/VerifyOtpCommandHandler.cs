using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Commands.VerifyOtp;

public class VerifyOtpCommandHandler(
    ICodeHasher codeHasher,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IClientContextProvider context,
    IVerificationCodeRepository verificationCodeRepository,
    IUserRepository userRepository,
    IDeviceRepository deviceRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ILogger<VerifyOtpCommandHandler> logger)
    : IRequestHandler<VerifyOtpCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var identifier = request.Email ?? request.PhoneNumber ?? "";

            var code = await verificationCodeRepository.GetUserCode(request.UserId, identifier, OtpPurpose.Registration,cancellationToken);

            if (code is null)
                return UserErrors.InvalidOrExpiredOtp;

            var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return UserErrors.InvalidOrExpiredOtp;

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

            var existingDevice = await deviceRepository.GetDeviceByIdAsync(request.DeviceId, cancellationToken);

            var userDevice = new UserDevice(request.UserId, request.DeviceId, user.PreferredLanguage, request.FcmToken);
            deviceRepository.AddDevice(userDevice);

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