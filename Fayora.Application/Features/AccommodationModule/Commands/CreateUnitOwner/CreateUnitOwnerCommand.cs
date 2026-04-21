using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public record CreateUnitOwnerCommand(
    UnitOwnerType OwnerType,
    string NationalIdUrl,
    string? CommercialName = null,
    string? TaxRegistrationNumber = null) : IRequest<Result<CreateUnitOwnerOwnerResult>>;