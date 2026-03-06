using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Interfaces;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IPasswordHasher passwordHasher,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public async Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsTracking = true, IncludeVerificationCodes = true };

        var existUser = await userRepository.GetUserByIdentityAsync(request.Identity, options, cancellationToken);

        var code = messageGenerator.GenerateCode();

        if (existUser is not null)
        {
            var statusCheck = existUser.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (existUser.IsVerified) return request.IsEmail ? AuthErrors.EmailAlreadyExists : AuthErrors.PhoneAlreadyExists;


            var check = request.IsEmail
                ? existUser.CanRequestEmailOtp()
                : existUser.CanRequestSmsOtp();

            if (check.IsError) return check.Errors;

            existUser.SendCode(request.Identity, code, OtpPurpose.Registration, codeHasher);

            existUser.ChangePassword(request.Password, passwordHasher);

            await unitOfWork.CommitChangesAsync(cancellationToken);
            return new RegisterResult(existUser.Id, request.Identity);
        }

        var userResult = User.Create(
            request.Email,
            request.PhoneNumber,
            request.Password,
            passwordHasher);

        if (userResult.IsError) return userResult.Errors;

        var newUser = userResult.Value;

        newUser.SendCode(request.Identity, code, OtpPurpose.Registration, codeHasher);

        userRepository.AddUser(newUser);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RegisterResult(newUser.Id, request.Identity);
    }
}