using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ResetPasswordEmail;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenHasher tokenHasher,
    IPasswordHasher passwordHasher)
    : ICommandHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var option = new UserQueryOptions { IsReadOnly = false };

        var user = request.Type == CodeDeliveryMethod.Email
            ? await userRepository.GetUserByEmailAsync(request.Value, option, cancellationToken)
            : await userRepository.GetUserByPhoneAsync(request.Value, option, cancellationToken);

        if (user is null) return AuthErrors.InvalidResetToken;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        var providedTokenHash = tokenHasher.HashToken(request.ResetToken);

        var resetToken = await userTokenRepository.GetTokenAsync(
            user.Id,
            TokenType.PasswordResetToken,
            providedTokenHash,
            cancellationToken,
            IsReadonly: false);

        if (resetToken is null || !resetToken.IsValid)
            return AuthErrors.InvalidResetToken;

        resetToken.Revoke();

        var resetResult = user.ChangePassword(request.NewPassword, passwordHasher);

        if (resetResult.IsError) return resetResult.Errors;

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}