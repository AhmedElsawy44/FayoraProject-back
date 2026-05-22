using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;

public record GetRecommendedGuidesQuery(
    int Count = 10) : IQuery<Result<List<RecommendedGuideResult>>>;
