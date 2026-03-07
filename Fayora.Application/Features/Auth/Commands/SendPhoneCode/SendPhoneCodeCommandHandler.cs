using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.SendPhoneCode;

public class SendPhoneCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator)
    : IRequestHandler<SendPhoneCodeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(SendPhoneCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = false, IncludeVerificationCodes = true };
        var user = await userRepository.GetUserByPhoneAsync(request.PhoneNumber, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        var canRequest = user.CanRequestPhoneCode();
        if (canRequest.IsError) return canRequest.Errors;

        string code = messageGenerator.GenerateCode();

        user.SendPhoneCode(request.PhoneNumber, code, request.Purpose, request.DeliveryMethod, codeHasher);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}