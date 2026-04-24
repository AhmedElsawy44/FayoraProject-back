using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteAccount;

public class VerifyDeleteAccountCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IClientContextProvider clientContextProvider
) : ICommandHandler<VerifyDeleteAccountCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(VerifyDeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();

        var userId = context.UserId;

        string target;

        if (request.Type == CodeDeliveryMethod.Email)
        {
            target = context.Email;

            if (string.IsNullOrWhiteSpace(target))
                return AuthErrors.EmailRequired;
        }
        else
        {
            target = context.PhoneNumber;

            if (string.IsNullOrWhiteSpace(target))
                return AuthErrors.PhoneNumberRequired;
        }

        var user = await userRepository.GetUserByIdAsync(
            userId,
            new UserQueryOptions { IsReadOnly = false },
            cancellationToken
        );

        if (user is null)
            return AuthErrors.UserNotFound;

        var check = user.CheckActiveStatus();
        if (check.IsError)
            return check.Errors;

        var vCode = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            target,
            CodePurpose.AccountDeletion,
            cancellationToken,
            isTracking: true
        );

        if (vCode is null)
            return AuthErrors.InvalidVerificationCode;

        var verifyResult = vCode.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
            return verifyResult.Errors;

        user.Delete();

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new Success();
    }
}