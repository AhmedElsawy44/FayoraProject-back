using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetSimilarGuides;

public class GetSimilarGuidesQueryHandler(
    IRecommendationService recommendationService)
    : IQueryHandler<GetSimilarGuidesQuery, Result<List<RecommendedGuideResult>>>
{
    public async Task<Result<List<RecommendedGuideResult>>> Handle(
        GetSimilarGuidesQuery request,
        CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var recommendations = await recommendationService.GetSimilarGuidesAsync(request.GuideId, count, cancellationToken);

        return Result<List<RecommendedGuideResult>>.CreateSuccess(recommendations);
    }
}
