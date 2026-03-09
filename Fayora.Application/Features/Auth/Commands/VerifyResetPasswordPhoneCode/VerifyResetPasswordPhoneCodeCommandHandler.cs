using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Shared.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordPhoneCode;

public class VerifyResetPasswordPhoneCodeCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenService userTokenService,
    IUserTokenRepository userTokens,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    ITokenHasher tokenHasher)
    : IRequestHandler<VerifyResetPasswordPhoneCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyResetPasswordPhoneCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = true };
        var user = await userRepository.GetUserByPhoneAsync(request.PhoneNumber, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        var resetPasswordOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.PhoneNumber,
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