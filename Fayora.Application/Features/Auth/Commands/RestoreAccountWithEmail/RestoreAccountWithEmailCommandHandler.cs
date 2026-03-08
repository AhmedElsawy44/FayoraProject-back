using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Commands.RestoreAccount;
using Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

public class RestoreAccountWithEmailCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<RestoreAccountWithEmailCommand, Result<RestoreAccountWithEmailResult>>
{
    public async Task<Result<RestoreAccountWithEmailResult>> Handle(
        RestoreAccountWithEmailCommand request,
        CancellationToken cancellationToken)
    {
        // 1 - Get the user
        var user = await userRepository.GetUserByEmailAsync(
            request.Email,
            new UserQueryOptions
            {
                IsReadOnly = false,
                IncludeVerificationCodes = true,
                IncludeRoles = true,
                UserStatus = UserStatus.Deleted
            },
            cancellationToken);

        if (user is null || !user.IsDeleted)
            return AuthErrors.UserNotFound;

        // 2 - Check OTP
        var otp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.Email,
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

        // 3 - Restore
        user.Restore();
        user.Login();

        // 4 - Upsert Device
        await userDeviceManager.UpsertDeviceAsync(
            user.Id,
            request.DeviceId,
            request.FcmToken,
            request.DeviceLanguage,
            cancellationToken);

        // 5 - Generate Tokens
        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RestoreAccountWithEmailResult(
            user.Id,
            request.Email,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}