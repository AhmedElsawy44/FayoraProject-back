using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuidesSeeAll;

public record GetRecommendedGuidesSeeAllQuery(
    int Page = 1,
    int Size = 10
) : IQuery<Result<List<RecommendedGuideResult>>>;
