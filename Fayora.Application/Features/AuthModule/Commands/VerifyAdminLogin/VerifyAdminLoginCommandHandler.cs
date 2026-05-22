using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyAdminLogin;

public class VerifyAdminLoginCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<VerifyAdminLoginCommand, Result<VerifyAdminLoginResult>>
{
    public async Task<Result<VerifyAdminLoginResult>> Handle(VerifyAdminLoginCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = string.Equals(request.Email, "Fayoratravel@gmail.com", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin) return AuthErrors.InvalidCredentials;

        var user = await userRepository.GetUserByEmailAsync(
            request.Email,
            new UserQueryOptions(IsReadOnly: false, IncludeVerificationCodes: true),
            cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.Roles is null || !user.Roles.Value.HasFlag(Role.Admin))
        {
            return AuthErrors.InvalidCredentials;
        }

        var loginOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.Email,
            CodePurpose.Login,
            cancellationToken);

        if (loginOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = loginOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.UpdateRegionalPreferences(request.SimCountryIsoCode, request.TimeZone);
        user.Login();

        await userDeviceManager.UpsertDeviceAsync(
            user.Id,
            request.DeviceId,
            request.FcmToken,
            request.DeviceLanguage,
            cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new VerifyAdminLoginResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.Email,
            user.ProfileImageUrl?.Value,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}
