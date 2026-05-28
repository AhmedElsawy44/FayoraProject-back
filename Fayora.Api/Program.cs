using Fayora.Api.Externals;
using Fayora.Api.Hubs;
using Fayora.Application;
using Fayora.Infrastructure;
using Microsoft.IdentityModel.JsonWebTokens;
using Scalar.AspNetCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace Fayora.Api;

public class Program
{
    public static async Task Main(string[] args)
    {


        Log.Logger = new LoggerConfiguration()
           .WriteTo.Console()
           .CreateBootstrapLogger();

        try
        {

            Log.Information("Starting Fayora API...");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Services
                    .AddPresentation()
                    .AddApplication()
                    .AddInfrastructure(builder.Configuration);
            }

            builder.Host.UseSerilog((context, services, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "logs/fayora-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7)
                    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]!));

            var app = builder.Build();

            // Auto-migrate
            await app.MigrateDatabaseAsync();

            //use background jobs (ex: Hangfire)
            app.UseBackgroundJobs();

            app.AddInfrastructureMiddleware();

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options => options
                    .WithTitle("Fayora API V1")
                    .WithTheme(ScalarTheme.Saturn)
                    .EnableDarkMode());

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fayora API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<ChatbotHub>("/chatbotHub");

            await app.RunAsync();
        }

        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }
}



