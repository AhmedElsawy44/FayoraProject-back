using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public class CreateUnitOwnerCommandHandler(
    IUnitOwnerRepository unitOwnerRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IAuthTokenGenerator authTokenGenerator) : ICommandHandler<CreateUnitOwnerCommand, Result<CreateUnitOwnerOwnerResult>>
{
    public async Task<Result<CreateUnitOwnerOwnerResult>> Handle(CreateUnitOwnerCommand request, CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var options = new UserQueryOptions { IsReadOnly = false };
        var user = await userRepository.GetUserByIdAsync(userId, options, cancellationToken);
        if (user is null) return AccommodationErrors.UserNotFound;

        if (user.Roles.HasFlag(Role.UnitOwner)) return AccommodationErrors.OwnerProfileAlreadyExists;


        if (await unitOwnerRepository.UnitOwnerExistAsync(userId, cancellationToken))
            return AccommodationErrors.OwnerProfileAlreadyExists;

        user.AddRole(Role.UnitOwner);

        UnitOwner unitOwner;

        if (request.OwnerType is UnitOwnerType.Commercial)
        {
            unitOwner = UnitOwner.CreateCommercialOwner(userId, request.CommercialName);
        }
        else
        {
            unitOwner = UnitOwner.CreateIndividualOwner(userId, request.CommercialName);
        }

        unitOwnerRepository.AddOwner(unitOwner);


        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateUnitOwnerOwnerResult(unitOwner.UserId, tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
    }
}