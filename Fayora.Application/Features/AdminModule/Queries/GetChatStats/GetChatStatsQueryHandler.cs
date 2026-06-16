using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Contracts.AdminModule.LiveChatMonitoring;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetChatStats;

public class GetChatStatsQueryHandler(IAdminRepository adminRepository) : IQueryHandler<GetChatStatsQuery, Result<GetChatStatsResponse>>
{
    public async Task<Result<GetChatStatsResponse>> Handle(GetChatStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await adminRepository.GetChatStatsAsync(cancellationToken);
        return stats;
    }
}
