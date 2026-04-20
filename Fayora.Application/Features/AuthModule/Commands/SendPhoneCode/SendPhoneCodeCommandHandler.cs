using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.SendPhoneCode;

public class SendPhoneCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator)
    : ICommandHandler<SendPhoneCodeCommand, Result<Unit>>
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