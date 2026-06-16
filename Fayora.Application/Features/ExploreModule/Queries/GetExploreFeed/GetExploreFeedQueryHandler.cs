using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ExploreModule;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.ExploreModule.Queries.GetExploreFeed;

public class GetExploreFeedQueryHandler(
    IExploreRepository exploreRepository,
    ITouristRepository touristRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetExploreFeedQuery, Result<List<ExploreItemResponse>>>
{
    public async Task<Result<List<ExploreItemResponse>>> Handle(
        GetExploreFeedQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch user wishlist if authenticated
        var wishlistIds = new List<Guid>();
        try
        {
            var context = clientContextProvider.GetContext();
            if (context != null && context.UserId != Guid.Empty)
            {
                var tourist = await touristRepository.GetTouristByUserIdAsync(context.UserId, true, cancellationToken);
                if (tourist != null)
                {
                    wishlistIds = tourist.WishIds.ToList();
                }
            }
        }
        catch
        {
            // Fail-safe for anonymous requests or context retrieval errors
        }

        // 2. Fetch raw pool from repository
        var itemsPool = await exploreRepository.GetExploreItemsAsync(
            request.Search,
            request.Type,
            wishlistIds,
            cancellationToken);

        List<ExploreItemResponse> resultList;

        // 3. Apply interleaving mixing only if no specific type is requested (i.e. "All")
        if (string.IsNullOrEmpty(request.Type) || request.Type.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var locations = itemsPool.Where(i => i.Type.Equals("location", StringComparison.OrdinalIgnoreCase)).ToList();
            var packages = itemsPool.Where(i => i.Type.Equals("package", StringComparison.OrdinalIgnoreCase)).ToList();
            var accommodations = itemsPool.Where(i => i.Type.Equals("accommodation", StringComparison.OrdinalIgnoreCase)).ToList();
            var guides = itemsPool.Where(i => i.Type.Equals("guide", StringComparison.OrdinalIgnoreCase)).ToList();

            var mixedList = new List<ExploreItemResponse>();
            int maxCount = Math.Max(
                Math.Max(locations.Count, packages.Count),
                Math.Max(accommodations.Count, guides.Count));

            for (int i = 0; i < maxCount; i++)
            {
                if (i < locations.Count) mixedList.Add(locations[i]);
                if (i < packages.Count) mixedList.Add(packages[i]);
                if (i < accommodations.Count) mixedList.Add(accommodations[i]);
                if (i < guides.Count) mixedList.Add(guides[i]);
            }

            resultList = mixedList;
        }
        else
        {
            resultList = itemsPool;
        }

        // 4. Paginate final list
        var paginatedResult = resultList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paginatedResult;
    }
}
