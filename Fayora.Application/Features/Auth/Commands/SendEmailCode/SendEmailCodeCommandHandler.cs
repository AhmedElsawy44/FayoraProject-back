using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.SendEmailCode;

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

        if (user.IsVerified && request.Purpose == CodePurpose.Registration)
            return AuthErrors.EmailIsAlreadyVerified;

        var canRequest = user.CanRequestEmailCode();
        if (canRequest.IsError) return canRequest.Errors;

        string code = messageGenerator.GenerateCode();

        user.SendEmailCode(request.Email, code, request.Purpose, codeHasher);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}