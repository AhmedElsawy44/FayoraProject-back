using Fayora.Api.Services;
using Fayora.Application.Common.Interfaces;

namespace Fayora.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddOpenApi();

        services.AddScoped<IClientContextProvider, ClientContextProvider>();

        return services;
    }
}