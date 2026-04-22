using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetAllMasterAmenities
{
    public class GetAllMasterAmenitiesQueryHandler(IMasterAmenityRepository masterAmenityRepository) : IRequestHandler<GetAllMasterAmenitiesQuery, GetAllMasterAmenitiesResult>
    {
        public async Task<GetAllMasterAmenitiesResult> Handle(GetAllMasterAmenitiesQuery request, CancellationToken cancellationToken)
        {
            var amenities = await masterAmenityRepository.GetAllAmenitiesAsync(cancellationToken);

            return new GetAllMasterAmenitiesResult(amenities);
        }
    }
}
