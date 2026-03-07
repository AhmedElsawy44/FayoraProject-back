using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenHasher tokenHasher,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var option = new UserQueryOptions { IsTracking = true };

        var user = await userRepository.GetUserByIdentityAsync(request.Identity, option, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        var providedTokenHash = tokenHasher.HashToken(request.ResetToken);

        var resetToken = await userTokenRepository.GetTokenAsync(user.Id, TokenType.PasswordResetToken, providedTokenHash, cancellationToken, isTracking: true);

        if (resetToken is null || !resetToken.IsValid)
            return AuthErrors.InvalidResetToken;

        resetToken.Revoke();

        var resetResult = user.ChangePassword(request.NewPassword, passwordHasher);

        if (resetResult.IsError) return resetResult.Errors;

        await unitOfWork.CommitChangesAsync(cancellationToken);

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        return Unit.Value;
    }
}