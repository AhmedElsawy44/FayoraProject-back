using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ChangeEmail;

public class ChangeEmailCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator,
    IPasswordHasher passwordHasher) : IRequestHandler<ChangeEmailCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false, IncludeVerificationCodes = true }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsCorrectPasswordHash(request.Password, passwordHasher)) return AuthErrors.InvalidPassword;

        if (user.PrimaryEmail?.Value == request.Email)
            return AuthErrors.EmailIsSameAsCurrent;

        if (await userRepository.IsEmailExistsAsync(request.Email, cancellationToken)) return AuthErrors.EmailAlreadyExists;

        var canRequest = user.CanRequestEmailCode();
        if (canRequest.IsError) return canRequest.Errors;

        string code = messageGenerator.GenerateCode();

        user.SendEmailCode(request.Email, code, CodePurpose.ChangeEmail, codeHasher);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
