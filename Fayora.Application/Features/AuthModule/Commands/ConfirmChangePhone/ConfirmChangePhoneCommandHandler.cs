using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public class ConfirmChangePhoneCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IAuthTokenGenerator authTokenGenerator)
    : ICommandHandler<ConfirmChangePhoneCommand, Result<ConfirmChangePhoneResult>>
{
    public async Task<Result<ConfirmChangePhoneResult>> Handle(ConfirmChangePhoneCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId,
        new UserQueryOptions
        {
            IsReadOnly = false
        },
        cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (await userRepository.IsPhoneNumberExistsAsync(request.NewPhoneNumber, cancellationToken))
            return AuthErrors.PhoneNumberAlreadyExists;

        var changePhoneOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id, request.NewPhoneNumber, CodePurpose.ChangePhoneNumber, cancellationToken);

        if (changePhoneOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = changePhoneOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        var changeResult = user.ChangePhoneNumber(request.NewPhoneNumber);
        if (changeResult.IsError) return changeResult.Errors;

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new ConfirmChangePhoneResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.NewPhoneNumber,
            user.ProfileImageUrl?.Value,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}