using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetAllAmenities;

public record GetAllAmenitiesQuery : ICommand<Result<List<AmenityResponse>>>;

public record AmenityResponse(
    int Id,
    string Name,
    string? IconUrl,
    string Category
);