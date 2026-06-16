using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Contracts.AdminModule.UpdateAccommodation;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedAccommodation;

public class GetDetailedAccommodationQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetDetailedAccommodationQuery, Result<GetDetailedAccommodationResponse>>
{
    public async Task<Result<GetDetailedAccommodationResponse>> Handle(
        GetDetailedAccommodationQuery request,
        CancellationToken cancellationToken)
    {
        var response = await adminRepository.GetDetailedAccommodationByIdAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return AccommodationErrors.UnitNotFound;
        }

        return response;
    }
}
