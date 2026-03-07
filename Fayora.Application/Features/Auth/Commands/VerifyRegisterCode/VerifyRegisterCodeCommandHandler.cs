using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.VerifyRegisterCode;

public class VerifyRegisterCodeCommandHandler(
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
    : IRequestHandler<VerifyRegisterCodeCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(VerifyRegisterCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsTracking = true, IncludeRoles = true };

        var user = await userRepository.GetUserByIdAsync(request.UserId, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsVerified) return AuthErrors.UserAccountIsAlreadyVerified;

        var registerOtp = await verificationCodeRepository.GetUserCodeAsync(user.Id, request.Identity, CodePurpose.Registration, cancellationToken);

        if (registerOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = registerOtp.Use(request.Code, codeHasher);

        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        if (request.IsEmail)
            user.VerifyEmail();
        else
            user.VerifyPhone();

        user.UpdateRegionalPreferences(request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);

        var existingDevice = await deviceRepository.GetDeviceByUserIdAndDeviceIdAsync(request.UserId, request.DeviceId, cancellationToken, isTracking: true);

        if (existingDevice is null)
        {
            var userDevice = new UserDevice(request.UserId, request.DeviceId, request.DeviceLanguage, request.FcmToken);
            deviceRepository.AddDevice(userDevice);
        }
        else
        {
            existingDevice.UpdateInfo(request.FcmToken, request.DeviceLanguage);
        }

        var roles = user.GetRoleNames();

        var accessToken = jwtService.GenerateToken(request.DeviceId, user, roles);

        var refreshTokenString = userTokenService.GenerateTokenString();
        var ipAddress = context.GetContext().IpAddress;

        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, request.DeviceId, TokenType.RefreshToken, cancellationToken);

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, request.DeviceId, ipAddress);
        userTokenRepository.AddToken(refreshToken);

        user.Login();

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new AuthResult(user.Id, request.Identity, accessToken, refreshTokenString);
    }
}