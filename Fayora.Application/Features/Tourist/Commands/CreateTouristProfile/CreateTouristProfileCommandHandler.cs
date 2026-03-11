using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Application.Features.Auth.Common;
using Fayora.Application.Features.Tourist.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Tourist;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.Tourist.Commands.CreateTouristProfile;

public class CreateTouristProfileCommandHandler(
    IUserRepository userRepository,
    ITouristRepository touristRepository,
    IMasterInterestRepository masterInterestRepository,
    IClientContextProvider clientContextProvider,
    IAuthTokenGenerator authTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTouristProfileCommand, Result<CreateTouristProfileResult>>
{
    public async Task<Result<CreateTouristProfileResult>> Handle(CreateTouristProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var options = new UserQueryOptions { IncludeRoles = true, IsReadOnly = true };

        var user = await userRepository.GetUserByIdAsync(userId, options, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var touristProfile = new TouristProfile(userId, request.BudgetTier, request.TravelStyle);

        if (request.InterestIds is not null && request.InterestIds.Count > 0)
        {
            if ((await masterInterestRepository.InterestsExistAsync(request.InterestIds, cancellationToken)) is false)
            {
                return TouristErrors.MasterInterestsNotFound;
            }

            touristProfile.AddInterests(request.InterestIds);
        }

        touristRepository.AddTourist(touristProfile);

        var tokens = await authTokenGenerator.GenerateTokensAsync(user, request.DeviceId, cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateTouristProfileResult(touristProfile.Id, tokens.AccessToken, tokens.RefreshToken);
    }
}
