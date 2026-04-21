using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IPasswordHasher passwordHasher) : ICommandHandler<ChangePasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId,
            new UserQueryOptions { IsReadOnly = false },
            cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;


        if (user.HasPassword)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) ||
                !user.IsCorrectPasswordHash(request.CurrentPassword, passwordHasher))
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return AuthErrors.InvalidPassword;
            }
        }

        var updateResult = user.ChangePassword(request.NewPassword, passwordHasher);

        if (updateResult.IsError) return updateResult.Errors;

        await userTokenRepository.RevokeAllTokensForUserAsync(user.Id, cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}