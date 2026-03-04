using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using Fayora.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public class VerifyResetPasswordOtpCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher) : IRequestHandler<VerifyResetPasswordOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyResetPasswordOtpCommand request, CancellationToken cancellationToken)
    {
        var identity = request.Email ?? request.PhoneNumber ?? "";

        var options = new UserQueryOptions(
            Status: AccountStatus.Verified,
            IsTracking: true,
            IncludeResetTokens: true,
            IncludeVerificationCodes: true
        );

        var user = await userRepository.GetUserByIdentityAsync(identity, options, cancellationToken);
        if (user is null) return AuthErrors.UserNotFound;


        var result = user.VerifyOtp(identity, request.Code, OtpPurpose.ResetPassword, codeHasher);

        if (result.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return result.Errors;
        }

        var token = user.GeneratePasswordResetToken(codeHasher);


        try
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // ده هيفهمنا مين الـ Entity اللي عامل الأزمة
            var entry = ex.Entries.Single();
            var clientValues = entry.Entity;
            throw new Exception($"Concurrency error on {clientValues.GetType().Name}. Make sure the ID is set correctly.");
        }

        return token;
    }
}