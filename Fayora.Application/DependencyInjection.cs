using Fayora.Application.Common.Behaviors;
using Fayora.Infrastructure.Services.AuthModule;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Fayora.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            options.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            options.AddOpenBehavior(typeof(LoggingBehavior<,>));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            options.AddOpenBehavior(typeof(BannedCheckBehavior<,>));
            options.AddOpenBehavior(typeof(PerformanceBehavior<,>));

        });

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<UploadStrategyFactory>();

        return services;
    }
}