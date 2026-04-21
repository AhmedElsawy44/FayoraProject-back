using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public class ConfirmChangeEmailCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IAuthTokenGenerator authTokenGenerator)
    : ICommandHandler<ConfirmChangeEmailCommand, Result<ConfirmChangeEmailResult>>
{
    public async Task<Result<ConfirmChangeEmailResult>> Handle(ConfirmChangeEmailCommand request, CancellationToken cancellationToken)
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

        if (await userRepository.IsEmailExistsAsync(request.NewEmail, cancellationToken))
            return AuthErrors.EmailAlreadyExists;

        var changeEmailOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id, request.NewEmail, CodePurpose.ChangeEmail, cancellationToken);

        if (changeEmailOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = changeEmailOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        var changeResult = user.ChangeEmail(request.NewEmail);
        if (changeResult.IsError) return changeResult.Errors;

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new ConfirmChangeEmailResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.NewEmail,
            user.ProfileImageUrl?.Value,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}