using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

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

        if (existUser is not null)
        {
            var statusCheck = existUser.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (existUser.IsVerified) return request.IsEmail ? AuthErrors.EmailAlreadyExists : AuthErrors.PhoneAlreadyExists;


            var check = request.IsEmail
                ? existUser.CanRequestEmailOtp()
                : existUser.CanRequestSmsOtp();

            if (check.IsError) return check.Errors;

            var codeForExistingUser = messageGenerator.GenerateCode();

            existUser.SendCode(request.Identity, codeForExistingUser, CodePurpose.Registration, request.DeliveryMethod, codeHasher);

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

        var codeForNewUser = messageGenerator.GenerateCode();

        newUser.SendCode(request.Identity, codeForNewUser, CodePurpose.Registration, request.DeliveryMethod, codeHasher);

        userRepository.AddUser(newUser);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RegisterResult(newUser.Id, request.Identity);
    }
}