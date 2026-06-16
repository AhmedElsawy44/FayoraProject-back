using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnitsSeeAll;

public record GetRecommendedUnitsSeeAllQuery(
    int Page = 1,
    int Size = 10
) : IQuery<Result<List<RecommendedUnitResult>>>;
