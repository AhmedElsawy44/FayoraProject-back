using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;
using MediatR;

namespace Fayora.Application.Features.Accommodation.Commands.CreateHousingOwner;

public record CreateHousingOwnerCommand(
    UnitOwnerType OwnerType,
    string NationalIdUrl,
    string? CommercialName = null,
    string? TaxRegistrationNumber = null) : IRequest<Result<CreateHousingOwnerResult>>;