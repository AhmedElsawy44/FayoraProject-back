using Fayora.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Fayora.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<EventualConsistencyMiddleware>();

        return builder;
    }
}