using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetUnitOwnerVerificationDetails;

public class GetUnitOwnerVerificationDetailsQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetUnitOwnerVerificationDetailsQuery, Result<GetUnitOwnerVerificationDetailsResponse>>
{
    public async Task<Result<GetUnitOwnerVerificationDetailsResponse>> Handle(
        GetUnitOwnerVerificationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await adminRepository.GetUnitOwnerVerificationDetailsAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return AccommodationErrors.OwnerProfileNotFound;
        }

        return response;
    }
}
