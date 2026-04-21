using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

public class RestoreAccountCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    ICodeHasher codeHasher,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<RestoreAccountCommand, Result<RestoreAccountResult>>
{
    public async Task<Result<RestoreAccountResult>> Handle(
        RestoreAccountCommand request,
        CancellationToken cancellationToken)
    {
        // 1 - Get the user
        var options = new UserQueryOptions
        {
            IsReadOnly = false,
            UserStatus = UserStatus.Deleted
        };

        var user = request.Type == CodeDeliveryMethod.Email
            ? await userRepository.GetUserByEmailAsync(request.Value, options, cancellationToken)
            : await userRepository.GetUserByPhoneAsync(request.Value, options, cancellationToken);

        if (user is null || !user.IsDeleted)
            return AuthErrors.UserNotFound;

        // 2 - Check OTP
        var otp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.Value,
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

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        // 5 - Generate Tokens

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RestoreAccountResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.Value,
            user.ProfileImageUrl?.Value,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}