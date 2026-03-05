using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICodeHasher codeHasher,
    IVerificationCodeService verificationCodeService,
    ILogger<ResendOtpCommandHandler> logger)
    : IRequestHandler<ResendOtpCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var identifier = request.Email ?? request.PhoneNumber ?? "";

            var options = new UserQueryOptions
            {
                IsTracking = true,
                IncludeVerificationCodes = true
            };

            var user = await userRepository.GetUserByIdentityAsync(identifier, options, cancellationToken);

            if (user is null)
                return AuthErrors.UserNotFound;

            var result = user.RequestOtp(identifier, request.OtpPurpose, verificationCodeService, codeHasher);

            if (result.IsError)
            {
                return result.Errors;
            }

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while resending OTP for User {Identifier}", request.Email ?? request.PhoneNumber);
            return Error.Failure("Server.Error", "An unexpected error occurred while processing your request.");
        }
    }
}