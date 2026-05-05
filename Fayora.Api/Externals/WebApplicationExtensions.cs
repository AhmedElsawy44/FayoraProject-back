using Fayora.Infrastructure.Jobs;
using Fayora.Infrastructure.Persistence.Repositories;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Api.Externals;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        return app;
    }

    public static WebApplication UseBackgroundJobs(this WebApplication app)
    {

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new HangfireAuthorizationFilter()]
        });


        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ExpiredBookingsJob>(
                "expire-pending-bookings",
                job => job.ExecuteAsync(CancellationToken.None),
                "*/5 * * * *");

        return app;
    }
}
