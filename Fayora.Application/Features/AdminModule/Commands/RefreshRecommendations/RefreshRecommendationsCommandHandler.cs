using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.RefreshRecommendations;

public class RefreshRecommendationsCommandHandler(
    IRecommendationService recommendationService)
    : ICommandHandler<RefreshRecommendationsCommand, Result<PythonRefreshResult>>
{
    public async Task<Result<PythonRefreshResult>> Handle(
        RefreshRecommendationsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await recommendationService.RefreshAsync(cancellationToken);
            return Result<PythonRefreshResult>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return Result<PythonRefreshResult>.CreateFailure(
                Error.Failure("Recommendation.RefreshFailed", $"Failed to refresh Python recommendation engine: {ex.Message}"));
        }
    }
}
