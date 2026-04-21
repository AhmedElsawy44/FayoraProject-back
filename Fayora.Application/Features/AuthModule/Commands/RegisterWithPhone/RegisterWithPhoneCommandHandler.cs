using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithPhone;

public class RegisterWithPhoneCommandHandler(
    IUserRepository userRepository,
    IMessageGenerator messageGenerator,
    IPasswordHasher passwordHasher,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterWithPhoneCommand, Result<RegisterWithPhoneResult>>
{

    public async Task<Result<RegisterWithPhoneResult>> Handle(RegisterWithPhoneCommand request, CancellationToken cancellationToken)
    {
        var options = new UserQueryOptions { IsReadOnly = false };

        var existUser = await userRepository.GetUserByPhoneAsync(request.PhoneNumber, options, cancellationToken);

        if (existUser is not null)
        {
            var statusCheck = existUser.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (existUser.IsVerified) return AuthErrors.PhoneAlreadyExists;

            var check = existUser.CanRequestPhoneCode();
            if (check.IsError) return check.Errors;

            var changePasswordResult = existUser.ChangePassword(request.Password, passwordHasher);
            if (changePasswordResult.IsError) return changePasswordResult.Errors;

            var codeForExistingUser = messageGenerator.GenerateCode();

            existUser.SendPhoneCode(request.PhoneNumber, codeForExistingUser, CodePurpose.VerifyAccount, request.DeliveryMethod, codeHasher);

            await unitOfWork.CommitChangesAsync(cancellationToken);
            return new RegisterWithPhoneResult(existUser.Id, request.PhoneNumber);
        }

        var userResult = User.CreateWithPhone(request.FirstName, request.LastName, request.PhoneNumber, request.Password, passwordHasher);

        if (userResult.IsError) return userResult.Errors;

        var newUser = userResult.Value;

        var codeForNewUser = messageGenerator.GenerateCode();

        newUser.SendPhoneCode(request.PhoneNumber, codeForNewUser, CodePurpose.VerifyAccount, request.DeliveryMethod, codeHasher);

        userRepository.AddUser(newUser);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new RegisterWithPhoneResult(newUser.Id, request.PhoneNumber);
    }
}
