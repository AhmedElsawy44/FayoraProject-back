using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryQueue;

public record GetInventoryQueueQuery(
    TypeFilter? TypeFilter,
    int Page = 1,
    int PageSize = 10
) : IQuery<List<InventoryQueueItemDto>>;
