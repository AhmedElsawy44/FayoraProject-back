using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : IRequestHandler<UpdateAccountCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var check = user.CheckActiveStatus();

        if (check.IsError) return check.Errors;

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.Gender,
            request.NationalityCode,
            request.ProfileImageUrl,
            request.Description,
            request.PreferredLanguage,
            request.TimeZone
        );

        await unitOfWork.CommitChangesAsync(cancellationToken);
        return new Success();
    }
}
