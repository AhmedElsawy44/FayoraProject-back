using Fayora.Api.Externals;
using Fayora.Api.Hubs;
using Fayora.Application;
using Fayora.Infrastructure;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;

namespace Fayora.Api;

public class Program
{
    //test a new repo in github  
    public static async Task Main(string[] args)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Services
                .AddPresentation()
                .AddApplication()
                .AddInfrastructure(builder.Configuration);
        }

        var app = builder.Build();

        // Auto-migrate
        await app.MigrateDatabaseAsync();

        //use background jobs (ex: Hangfire)
        app.UseBackgroundJobs();

        app.AddInfrastructureMiddleware();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fayora API V1");
                c.RoutePrefix = "swagger";
            });
            app.MapOpenApi();
        }

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/chatHub");

        await app.RunAsync();
    }
}
