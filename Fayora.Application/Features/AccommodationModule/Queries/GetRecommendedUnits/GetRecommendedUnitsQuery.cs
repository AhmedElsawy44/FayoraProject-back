using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;

/// <summary>
/// Query to retrieve recommended housing units for the homepage.
/// Works for both authenticated users (personalized) and anonymous users (trending).
/// </summary>
public record GetRecommendedUnitsQuery(
    int Count = 10
) : IQuery<Result<List<RecommendedUnitResult>>>;
