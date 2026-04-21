using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<VerifyPhoneCommand, Result<VerifyPhoneResult>>
{
    public async Task<Result<VerifyPhoneResult>> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByPhoneAsync(
            request.PhoneNumber,
            new UserQueryOptions { IsReadOnly = false },
            cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsPhoneVerified) return AuthErrors.PhoneIsAlreadyVerified;

        var registerOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.PhoneNumber,
            CodePurpose.VerifyAccount,
            cancellationToken);

        if (registerOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = registerOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.VerifyPhone();
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

        return new VerifyPhoneResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.PhoneNumber,
            user.ProfileImageUrl?.Value,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}