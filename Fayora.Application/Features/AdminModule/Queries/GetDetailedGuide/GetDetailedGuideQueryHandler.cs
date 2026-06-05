using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedGuide;

public class GetDetailedGuideQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetDetailedGuideQuery, Result<GetDetailedGuideResponse>>
{
    public async Task<Result<GetDetailedGuideResponse>> Handle(
        GetDetailedGuideQuery request,
        CancellationToken cancellationToken)
    {
        var response = await adminRepository.GetDetailedGuideByIdAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return AccommodationErrors.UserNotFound;
        }

        return response;
    }
}
