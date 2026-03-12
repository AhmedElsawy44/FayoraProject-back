using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Application.Features.Accommodation.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.Enums.AccommodationModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.Accommodation.Commands.CreateHousingOwner;

public class CreateHousingOwnerCommandHandler(
    IUnitOwnerRepository unitOwnerRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IAuthTokenGenerator authTokenGenerator) : IRequestHandler<CreateHousingOwnerCommand, Result<CreateHousingOwnerResult>>
{
    public async Task<Result<CreateHousingOwnerResult>> Handle(CreateHousingOwnerCommand request, CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;
        var deviceId = context.DeviceId;

        var options = new UserQueryOptions { IncludeRoles = true, IsReadOnly = false };
        var user = await userRepository.GetUserByIdAsync(userId, options, cancellationToken);
        if (user is null) return AccommodationErrors.UserNotFound;

        if (user.Roles.Any()) return AccommodationErrors.UserAlreadyHasRole;

        var existingOwner = await unitOwnerRepository.GetOwnerByUserIdAsync(userId, isReadOnly: true, cancellationToken);
        if (existingOwner is not null) return AccommodationErrors.OwnerProfileAlreadyExists;

        UnitOwner unitOwner;

        if (request.OwnerType is UnitOwnerType.Commercial)
        {
            unitOwner = UnitOwner.CreateCommercialOwner(userId, request.NationalIdUrl, request.CommercialName!, request.TaxRegistrationNumber);
        }
        else
        {
            unitOwner = UnitOwner.CreateIndividualOwner(userId, request.NationalIdUrl);
        }

        unitOwnerRepository.AddOwner(unitOwner);

        var ownerRole = await roleRepository.GetRoleByNameAsync("Owner", cancellationToken);
        if (ownerRole is null) return AccommodationErrors.RoleNotFound;

        user.AddRole(ownerRole);

        var tokens = await authTokenGenerator.GenerateTokensAsync(user, deviceId, cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateHousingOwnerResult(unitOwner.Id, tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
    }
}