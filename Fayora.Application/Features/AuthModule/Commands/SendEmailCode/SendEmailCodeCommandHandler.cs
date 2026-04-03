using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.SendEmailCode;

public class SendEmailCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator)
    : IRequestHandler<SendEmailCodeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(SendEmailCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = false, IncludeVerificationCodes = true };
        var user = await userRepository.GetUserByEmailAsync(request.Email, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsVerified && request.Purpose == CodePurpose.VerifyAccount)
            return AuthErrors.EmailIsAlreadyVerified;

        var canRequest = user.CanRequestEmailCode();
        if (canRequest.IsError) return canRequest.Errors;

        string code = messageGenerator.GenerateCode();

        user.SendEmailCode(request.Email, code, request.Purpose, codeHasher);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}