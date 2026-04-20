using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public class ConfirmChangePhoneCommandHandlerIUserRepository(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IJwtService jwtService)
    : ICommandHandler<ConfirmChangePhoneCommand, Result<ConfirmChangePhoneResult>>
{
    public async Task<Result<ConfirmChangePhoneResult>> Handle(ConfirmChangePhoneCommand request, CancellationToken cancellationToken)
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

        var changePhoneOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id, request.NewPhoneNumber, CodePurpose.ChangePhoneNumber, cancellationToken);

        if (changePhoneOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = changePhoneOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.ChangePhoneNumber(request.NewPhoneNumber);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(
            request.DeviceId,
            user);

        return new ConfirmChangePhoneResult(user.Id, request.NewPhoneNumber, token, jwtService.ExpiresIn);
    }
}
