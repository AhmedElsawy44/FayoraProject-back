using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Interfaces;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICodeHasher codeHasher, IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var identity = request.Email ?? request.PhoneNumber ?? "";

        var option = new UserQueryOptions
        {
            Status = AccountStatus.Verified,
            IncludeResetTokens = true,
            IsTracking = true
        };

        var user = await userRepository.GetUserByIdentityAsync(identity, option, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var result = user.ResetPassword(request.ResetToken, request.NewPassword, codeHasher, passwordHasher);

        if (result.IsError) return result.Errors;

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}