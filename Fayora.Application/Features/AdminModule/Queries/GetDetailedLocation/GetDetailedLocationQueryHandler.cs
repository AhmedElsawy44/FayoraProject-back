using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Contracts.AdminModule.UpdateLocation;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedLocation;

public class GetDetailedLocationQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetDetailedLocationQuery, Result<GetDetailedLocationResponse>>
{
    public async Task<Result<GetDetailedLocationResponse>> Handle(
        GetDetailedLocationQuery request,
        CancellationToken cancellationToken)
    {
        var response = await adminRepository.GetDetailedLocationByIdAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return Error.NotFound("Location.NotFound", "Location not found.");
        }

        return response;
    }
}
