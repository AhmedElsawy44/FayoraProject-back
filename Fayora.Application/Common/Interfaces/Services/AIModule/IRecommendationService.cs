namespace Fayora.Application.Common.Interfaces.Services.AIModule;

public interface IRecommendationService
{
    Task<IEnumerable<T>> FilterAndRankAsync<T>(IEnumerable<T> rawResults, string? userId);
}