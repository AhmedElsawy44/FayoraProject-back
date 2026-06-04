using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.EvaluateRecommendations;

public class EvaluateRecommendationsQueryHandler(
    IRecommendationService recommendationService)
    : IQueryHandler<EvaluateRecommendationsQuery, Result<object>>
{
    public async Task<Result<object>> Handle(
        EvaluateRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await recommendationService.EvaluateAsync(request.TopN, request.CutoffDate, cancellationToken);
            return Result<object>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return Result<object>.CreateFailure(
                Error.Failure("Recommendation.EvaluationFailed", $"Failed to evaluate Python recommendation engine: {ex.Message}"));
        }
    }
}
