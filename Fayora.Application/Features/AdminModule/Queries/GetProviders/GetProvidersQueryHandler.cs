using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetProviders;

public class GetProvidersQueryHandler(IAdminRepository adminRepository)
    : IQueryHandler<GetProvidersQuery, Result<List<GetProvidersResponse>>>
{
    public async Task<Result<List<GetProvidersResponse>>> Handle(
        GetProvidersQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await adminRepository.GetProvidersAsync(
            request.ProviderType,
            request.PageNumber,
            request.PageSize,
            request.SearchQuery,
            cancellationToken);

        return providers;
    }
}
