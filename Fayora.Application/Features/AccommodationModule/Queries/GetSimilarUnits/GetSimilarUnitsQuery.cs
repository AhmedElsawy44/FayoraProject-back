using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetSimilarUnits;

public record GetSimilarUnitsQuery(
    Guid UnitId,
    int Count = 5
) : IQuery<Result<List<RecommendedUnitResult>>>;
