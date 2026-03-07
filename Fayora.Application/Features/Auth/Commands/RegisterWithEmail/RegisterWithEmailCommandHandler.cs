using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.RegisterWithEmail;

public class RegisterWithEmailCommandHandler(
    IPasswordHasher passwordHasher,
    ICodeHasher codeHasher,
    IMessageGenerator messageGenerator,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterWithEmailCommand, Result<RegisterWithEmailResult>>
{
    public async Task<Result<RegisterWithEmailResult>> Handle(RegisterWithEmailCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = false, IncludeVerificationCodes = true };

        var existUser = await userRepository.GetUserByEmailAsync(request.Email, options, cancellationToken);

        if (existUser is not null)
        {
            var statusCheck = existUser.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (existUser.IsVerified) return AuthErrors.EmailAlreadyExists;


            var check = existUser.CanRequestEmailCode();

            if (check.IsError) return check.Errors;

            var codeForExistingUser = messageGenerator.GenerateCode();

            existUser.SendEmailCode(request.Email, codeForExistingUser, CodePurpose.Registration, codeHasher);

            existUser.ChangePassword(request.Password, passwordHasher);

            await unitOfWork.CommitChangesAsync(cancellationToken);
            return new RegisterWithEmailResult(existUser.Id, request.Email);
        }

        var userResult = User.CreateWithEmail(request.Email, request.Password, passwordHasher);

        if (userResult.IsError) return userResult.Errors;

        var newUser = userResult.Value;

        var codeForNewUser = messageGenerator.GenerateCode();

        newUser.SendEmailCode(request.Email, codeForNewUser, CodePurpose.Registration, codeHasher);

        userRepository.AddUser(newUser);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RegisterWithEmailResult(newUser.Id, request.Email);
    }
}
