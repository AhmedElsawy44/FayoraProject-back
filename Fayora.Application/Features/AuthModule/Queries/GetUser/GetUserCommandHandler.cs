using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public class GetUserCommandHandler(
    IUserRepository userRepository,
    IClientContextProvider clientContextProvider
    ) : IRequestHandler<GetUserCommand, Result<GetUserResult>>
{
    public async Task<Result<GetUserResult>> Handle(GetUserCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = true }, cancellationToken);

        if (user == null) return AuthErrors.UserNotFound;

        return new GetUserResult
        (
            user.Id,
            user.ProfileImageUrl,
            user.PrimaryEmail?.Value,
            user.PhoneNumber,
            user.IsEmailVerified,
            user.IsPhoneVerified,
            user.FirstName,
            user.LastName,
            user.NationalityCode,
            user.Gender,
            user.BirthDate,
            user.Description,
            user.PreferredLanguage,
            user.UserLanguageProficiency
        );
    }
}
