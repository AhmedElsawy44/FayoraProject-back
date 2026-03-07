using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.SendCode;

public class SendCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator)
    : IRequestHandler<SendCodeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsTracking = true, IncludeVerificationCodes = true };

        var user = await userRepository.GetUserByIdentityAsync(request.Identity, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsVerified && request.OtpPurpose == CodePurpose.Registration) return AuthErrors.UserAccountIsAlreadyVerified;

        if (!user.IsVerified && request.OtpPurpose == CodePurpose.ResetPassword) return AuthErrors.UserNotVerified;

        var check = request.IsEmail
                ? user.CanRequestEmailOtp()
                : user.CanRequestSmsOtp();

        if (check.IsError) return check.Errors;

        string code = messageGenerator.GenerateCode();

        user.SendCode(request.Identity, code, request.OtpPurpose, request.DeliveryMethod, codeHasher);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}