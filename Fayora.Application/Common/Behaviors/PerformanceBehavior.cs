using Fayora.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Fayora.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        var requestName = typeof(TRequest).Name;

        if (elapsedMilliseconds > 500)
        {
            logger.LogWarning(
                "Long Running Request: {RequestName} ({ElapsedMilliseconds} ms) {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }

        return response;
    }
}