using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Interfaces;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordCode;

public class VerifyResetPasswordCodeCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserTokenService userTokenService,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher)
    : IRequestHandler<VerifyResetPasswordCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsTracking = true, IncludeVerificationCodes = true };

        var user = await userRepository.GetUserByIdentityAsync(request.Identity, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        if (user.IsLocked) return Error.Failure("User.UserLocked", $"This user account is locked until {user.LockedUntil?.ToString("u")}.");

        if (user.IsDeleted) return AuthErrors.UserDeleted;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        var resetPasswordOtp = await verificationCodeRepository.GetUserCodeAsync(user.Id, request.Identity, OtpPurpose.ResetPassword, cancellationToken);

        if (resetPasswordOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = resetPasswordOtp.Use(request.Code, codeHasher);

        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        var rawToken = userTokenService.GenerateTokenString();

        var hashedToken = codeHasher.HashCode(rawToken);

        var token = UserTokens.PasswordResetToken(user.Id, hashedToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return rawToken;
    }
}