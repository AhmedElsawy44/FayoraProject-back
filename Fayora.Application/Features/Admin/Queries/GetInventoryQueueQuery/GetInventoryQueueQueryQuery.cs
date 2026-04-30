using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;

namespace Fayora.Application.Features.Admin.Queries.GetInventoryQueueQuery;

public record GetInventoryQueueQuery(
    TypeFilter? TypeFilter,
    int Page = 1,
    int PageSize = 10
) : IQuery<List<InventoryQueueItemDto>>;
