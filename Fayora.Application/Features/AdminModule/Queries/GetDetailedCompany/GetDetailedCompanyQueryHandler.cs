using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedCompany;

public class GetDetailedCompanyQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetDetailedCompanyQuery, Result<GetDetailedCompanyResponse>>
{
    public async Task<Result<GetDetailedCompanyResponse>> Handle(
        GetDetailedCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var response = await adminRepository.GetDetailedCompanyByIdAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return AccommodationErrors.UserNotFound; // or general not found
        }

        return response;
    }
}
