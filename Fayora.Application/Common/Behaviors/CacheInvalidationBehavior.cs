using Fayora.Application.Common.Abstractions.Caching;
using MediatR;

namespace Fayora.Application.Common.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheInvalidatorCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        await cacheService.RemoveByPrefixAsync(request.CacheKeyToClear, cancellationToken);

        return response;
    }
}
