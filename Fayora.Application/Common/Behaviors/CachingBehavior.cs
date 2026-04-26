using Fayora.Application.Common.Abstractions.Caching;
using MediatR;

namespace Fayora.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
    where TResponse : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;
        if (string.IsNullOrEmpty(cacheKey))
        {
            return await next(cancellationToken);
        }
        var cachedResponse = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        var response = await next(cancellationToken);
        await cacheService.SetAsync(cacheKey, response, request.Expiration, cancellationToken);
        return response;
    }
}
