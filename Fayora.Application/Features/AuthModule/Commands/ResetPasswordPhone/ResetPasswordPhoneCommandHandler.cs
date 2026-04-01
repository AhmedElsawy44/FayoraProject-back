using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ResetPasswordPhone;

public class ResetPasswordPhoneCommandHandler( 
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenHasher tokenHasher,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordPhoneCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordPhoneCommand request, CancellationToken cancellationToken)
    {
        var option = new UserQueryOptions { IsReadOnly = false };

        var user = await userRepository.GetUserByPhoneAsync(request.PhoneNumber, option, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        var providedTokenHash = tokenHasher.HashToken(request.ResetToken);

        var resetToken = await userTokenRepository.GetTokenAsync(user.Id, TokenType.PasswordResetToken, providedTokenHash, cancellationToken, IsReadonly: false);

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