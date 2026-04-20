using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteEmailAccount;

public class VerifyDeleteEmailAccountCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IClientContextProvider clientContextProvider) : ICommandHandler<VerifyDeleteEmailAccountCommand, Result<Success>>
{

    public async Task<Result<Success>> Handle(VerifyDeleteEmailAccountCommand request, CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();

        var userId = context.UserId;

        var email = context.Email;

        if (string.IsNullOrWhiteSpace(context.Email)) return AuthErrors.EmailRequired;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var check = user.CheckActiveStatus();

        if (check.IsError) return check.Errors;

        var vCode = await verificationCodeRepository.GetUserCodeAsync(user.Id, email, CodePurpose.AccountDeletion, cancellationToken, isTracking: true);

        if (vCode is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = vCode.Use(request.Code, codeHasher);

        if (verifyResult.IsError) return verifyResult.Errors;

        user.Delete();
        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new Success();
    }
}
