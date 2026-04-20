using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordEmailCode;

public class VerifyResetPasswordEmailCodeCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenService userTokenService,
    IUserTokenRepository userTokens,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    ITokenHasher tokenHasher)
    : ICommandHandler<VerifyResetPasswordEmailCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyResetPasswordEmailCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = true };
        var user = await userRepository.GetUserByEmailAsync(request.Email, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;


        var resetPasswordOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.Email,
            CodePurpose.ResetPassword,
            cancellationToken);

        if (resetPasswordOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = resetPasswordOtp.Use(request.Code, codeHasher);

        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        var rawToken = userTokenService.GenerateTokenString();
        var hashedToken = tokenHasher.HashToken(rawToken);
        var token = UserTokens.PasswordResetToken(user.Id, hashedToken);

        userTokens.AddToken(token);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return rawToken;
    }
}