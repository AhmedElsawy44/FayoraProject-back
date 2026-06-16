using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuidesSeeAll;

public class GetRecommendedGuidesSeeAllQueryHandler(
    IRecommendationService recommendationService,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetRecommendedGuidesSeeAllQuery, Result<List<RecommendedGuideResult>>>
{
    public async Task<Result<List<RecommendedGuideResult>>> Handle(
        GetRecommendedGuidesSeeAllQuery request,
        CancellationToken cancellationToken)
    {
        var context = clientContextProvider.GetContext();
        var userId = context.UserId;

        var page = Math.Max(request.Page, 1);
        var size = Math.Clamp(request.Size, 1, 50);

        var recommendations = await recommendationService.GetGuidesSeeAllAsync(userId, page, size, cancellationToken);

        return Result<List<RecommendedGuideResult>>.CreateSuccess(recommendations);
    }
}
