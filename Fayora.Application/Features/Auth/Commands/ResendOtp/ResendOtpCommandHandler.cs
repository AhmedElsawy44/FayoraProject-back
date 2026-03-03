using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Interfaces;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResendRegisterOtp;

public class ResendOtpCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IVerificationCodeRepository verificationCodeRepository, ICodeHasher codeHasher, IVerificationCodeService verificationCodeService) : IRequestHandler<ResendOtpCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken, isTracking: true);

        if (user is null)
            return AuthErrors.UserNotFound;

        var identifier = request.Email ?? request.PhoneNumber;
        var result = user.RequestOtp(identifier!, request.SimCountryIsoCode, request.OtpPurpose, verificationCodeService, codeHasher);
        if (result.IsError)
        {
            return result.Errors;
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
