using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler(
    ICodeHasher codeHasher,
    IJwtService jwtService,
    ITokenHasher tokenHasher,
    IUserTokenService userTokenService,
    IClientContextProvider context,
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IDeviceRepository deviceRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyPhoneCommand, Result<VerifyPhoneResult>>
{
    public async Task<Result<VerifyPhoneResult>> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(request.UserId, new UserQueryOptions { IsReadOnly = false, IncludeRoles = true }, cancellationToken);
        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsPhoneVerified) return AuthErrors.PhoneIsAlreadyVerified;

        var registerOtp = await verificationCodeRepository.GetUserCodeAsync(user.Id, request.PhoneNumber, CodePurpose.Registration, cancellationToken);
        if (registerOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = registerOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.VerifyPhone();
        user.UpdateRegionalPreferences(request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);

        var existingDevice = await deviceRepository.GetDeviceByUserIdAndDeviceIdAsync(request.UserId, request.DeviceId, cancellationToken, isTracking: true);

        if (existingDevice is null)
        {
            deviceRepository.AddDevice(new UserDevice(request.UserId, request.DeviceId, request.DeviceLanguage, request.FcmToken));
        }
        else
        {
            existingDevice.UpdateInfo(request.FcmToken, request.DeviceLanguage);
        }

        var accessToken = jwtService.GenerateToken(request.DeviceId, user);

        var refreshTokenString = userTokenService.GenerateTokenString();
        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);
        var ipAddress = context.GetContext().IpAddress;

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, request.DeviceId, TokenType.RefreshToken, cancellationToken);

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, request.DeviceId, ipAddress);
        userTokenRepository.AddToken(refreshToken);

        user.Login();
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new VerifyPhoneResult(
            user.Id,
            request.PhoneNumber,
            accessToken,
            refreshTokenString,
            jwtService.ExpiresIn);
    }
}