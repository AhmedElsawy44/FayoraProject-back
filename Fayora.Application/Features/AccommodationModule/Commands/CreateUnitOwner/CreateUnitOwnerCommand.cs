using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public record CreateUnitOwnerCommand
(
    string DeviceId,
    UnitOwnerType OwnerType,
    string CommercialName
) : ICommand<Result<CreateUnitOwnerOwnerResult>>;