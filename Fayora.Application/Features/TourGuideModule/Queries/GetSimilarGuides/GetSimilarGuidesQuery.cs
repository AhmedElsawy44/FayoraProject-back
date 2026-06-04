using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetSimilarGuides;

public record GetSimilarGuidesQuery(
    Guid GuideId,
    int Count = 5
) : IQuery<Result<List<RecommendedGuideResult>>>;
