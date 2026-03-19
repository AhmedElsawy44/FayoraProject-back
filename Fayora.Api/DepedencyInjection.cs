using Fayora.Api.Services;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using System.Text.Json.Serialization;

namespace Fayora.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers()
          .AddJsonOptions(options =>
          {
              options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
              options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
          });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddOpenApi();

        services.AddScoped<IClientContextProvider, ClientContextProvider>();

        return services;
    }
}