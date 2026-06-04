using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.EvaluateRecommendations;

public record EvaluateRecommendationsQuery(
    int TopN = 10,
    string CutoffDate = "2025-01-01"
) : IQuery<Result<object>>;
