using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.RegisterWithPhone
{
    public class RegisterWithPhoneCommandHandler(IUserRepository userRepository, IMessageGenerator messageGenerator, IPasswordHasher passwordHasher, ICodeHasher codeHasher, IUnitOfWork unitOfWork) : IRequestHandler<RegisterWithPhoneCommand, Result<RegisterWithPhoneResult>>
    {

        public async Task<Result<RegisterWithPhoneResult>> Handle(RegisterWithPhoneCommand request, CancellationToken cancellationToken)
        {
            var options = new UserQueryOptions { IsReadOnly = false, IncludeVerificationCodes = true };

            var existUser = await userRepository.GetUserByPhoneAsync(request.PhoneNumber, options, cancellationToken);

            if (existUser is not null)
            {
                var statusCheck = existUser.CheckActiveStatus();
                if (statusCheck.IsError) return statusCheck.Errors;

                if (existUser.IsVerified) return AuthErrors.PhoneAlreadyExists;


                var check = existUser.CanRequestPhoneCode();

                if (check.IsError) return check.Errors;

                var codeForExistingUser = messageGenerator.GenerateCode();

                existUser.SendPhoneCode(request.PhoneNumber, codeForExistingUser, CodePurpose.Registration, request.DeliveryMethod, codeHasher);

                existUser.ChangePassword(request.Password, passwordHasher);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new RegisterWithPhoneResult(existUser.Id, request.PhoneNumber);
            }

            var userResult = User.CreateWithPhone(request.PhoneNumber, request.Password, passwordHasher);

            if (userResult.IsError) return userResult.Errors;

            var newUser = userResult.Value;

            var codeForNewUser = messageGenerator.GenerateCode();

            newUser.SendPhoneCode(request.PhoneNumber, codeForNewUser, CodePurpose.Registration, request.DeliveryMethod, codeHasher);

            userRepository.AddUser(newUser);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new RegisterWithPhoneResult(newUser.Id, request.PhoneNumber);
        }
    }
}
