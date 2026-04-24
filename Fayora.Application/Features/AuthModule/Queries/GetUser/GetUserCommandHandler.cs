using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public class GetUserCommandHandler(
    IUserRepository userRepository,
    IClientContextProvider clientContextProvider
    ) : IQueryHandler<GetUserQuery, Result<GetUserResult>>
{
    public async Task<Result<GetUserResult>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = true }, cancellationToken);

        if (user == null) return AuthErrors.UserNotFound;

        return new GetUserResult
        (
            user.Id,
            user.ProfileImageUrl?.Value,
            user.PrimaryEmail?.Value,
            user.PhoneNumber?.Value,
            user.IsEmailVerified,
            user.IsPhoneVerified,
            user.FirstName,
            user.LastName,
            user.NationalityCode,
            user.Gender,
            user.BirthDate,
            user.Description,
            user.PreferredLanguage,
            user.UserLanguageProficiencies?.ToList() ?? []
        );
    }
}
