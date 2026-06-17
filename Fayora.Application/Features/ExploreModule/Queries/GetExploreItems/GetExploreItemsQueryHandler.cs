using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ExploreModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ExploreModule.Queries.GetExploreItems;

public class GetExploreItemsQueryHandler(
    IExploreRepository exploreRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetExploreItemsQuery, Result<List<ExploreItemDto>>>
{
    public async Task<Result<List<ExploreItemDto>>> Handle(
        GetExploreItemsQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var items = await exploreRepository.GetExploreItemsAsync(
            userId,
            request.PageNumber,
            request.PageSize,
            request.Search,
            cancellationToken);

        return items;
    }
}
