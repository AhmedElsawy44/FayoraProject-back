using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone
{
    public class ChangePhoneCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IClientContextProvider clientContextProvider,
        ICodeHasher codeHasher,
        IMessageGenerator messageGenerator,
        IPasswordHasher passwordHasher) : IRequestHandler<ChangePhoneCommand, Result<Unit>>
    {

        public async Task<Result<Unit>> Handle(ChangePhoneCommand request, CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IncludeRoles = true, IsReadOnly = false }, cancellationToken);

            if (user is null) return AuthErrors.UserNotFound;

            var statusCheck = user.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (!user.IsCorrectPasswordHash(request.Password, passwordHasher)) return AuthErrors.InvalidPassword;

            if (user.PhoneNumber == request.PhoneNumber)
                return AuthErrors.PhoneIsSameAsCurrent;

            if (await userRepository.IsPhoneNumberExistsAsync(request.PhoneNumber, cancellationToken)) return AuthErrors.PhoneNumberAlreadyExists;

            var canRequest = user.CanRequestEmailCode();
            if (canRequest.IsError) return canRequest.Errors;

            string code = messageGenerator.GenerateCode();

            user.SendPhoneCode(request.PhoneNumber, code, CodePurpose.ChangePhoneNumber, request.DeliveryMethod, codeHasher);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
