using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedLocations;

/// <summary>
/// Query to retrieve recommended locations for the homepage.
/// Works for both authenticated users (personalized) and anonymous users (trending).
/// </summary>
public record GetRecommendedLocationsQuery(
    int Count = 10
) : IQuery<Result<List<RecommendedLocationResult>>>;
