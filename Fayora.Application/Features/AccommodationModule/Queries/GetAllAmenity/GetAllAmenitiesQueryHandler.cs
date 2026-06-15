using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetAllAmenities;

public class GetAllAmenitiesQueryHandler(IMasterAmenityRepository masterAmenityRepository)
    : ICommandHandler<GetAllAmenitiesQuery, Result<List<AmenityResponse>>>
{
    public async Task<Result<List<AmenityResponse>>> Handle(
        GetAllAmenitiesQuery request,
        CancellationToken cancellationToken)
    {
        var amenities = await masterAmenityRepository.GetAllAsync(true, cancellationToken);

        var response = amenities.Select(a => new AmenityResponse(
            a.Id,
            a.Name,
            a.Icon.Value,
            a.Category.ToString()
        )).ToList();

        return response;
    }
}