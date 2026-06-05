using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ExploreModule.Queries.GetExploreItems;

public record GetExploreItemsQuery(
    int PageNumber,
    int PageSize
) : IQuery<Result<List<ExploreItemDto>>>;
