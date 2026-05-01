using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryQueueQuery;

public class GetInventoryQueueQueryHandler(IInventoryModerationService inventoryModerationService)
    : IQueryHandler<GetInventoryQueueQuery, List<InventoryQueueItemDto>>
{
    public async Task<List<InventoryQueueItemDto>> Handle(GetInventoryQueueQuery request, CancellationToken ct)
    {
        var items = await inventoryModerationService.GetInventoryQueueAsync(
            request.TypeFilter,
            request.Page,
            request.PageSize,
            ct);

        return items;
    }
}

