using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllRecommendations;

public record GetAllRecommendationsQuery(
    int Count = 5
) : IQuery<Result<AllRecommendationsResult>>;
