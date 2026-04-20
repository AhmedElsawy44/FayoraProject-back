using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public class ConfirmChangeEmailCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IJwtService jwtService)
    : ICommandHandler<ConfirmChangeEmailCommand, Result<ConfirmChangeEmailResult>>
{
    public async Task<Result<ConfirmChangeEmailResult>> Handle(ConfirmChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId,
        new UserQueryOptions
        {
            IsReadOnly = false,
            IncludeVerificationCodes = true
        },
        cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

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

        await unitOfWork.CommitChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(
            request.DeviceId,
            user);

        return new ConfirmChangeEmailResult(user.Id, request.NewEmail, token, jwtService.ExpiresIn);
    }
}