using Fayora.Domain.Common.Constants;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Infrastructure.Jobs;
using Fayora.Infrastructure.Persistence.Repositories;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Api.Externals;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                var checkAndBaselineSql = @"
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
    BEGIN
        CREATE TABLE [__EFMigrationsHistory] (
            [MigrationId] nvarchar(150) NOT NULL CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY,
            [ProductVersion] nvarchar(32) NOT NULL
        );
    END
    
    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260601085713_AddOffersSystem')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20260601085713_AddOffersSystem', '10.0.0');
    END
END";

                await dbContext.Database.ExecuteSqlRawAsync(checkAndBaselineSql, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to execute migration baseline check: {ex.Message}");
        }

        await dbContext.Database.MigrateAsync(cancellationToken);

        // Seed Chatbot User if not exists
        var botUserId = ChatbotConstants.BotUserId;
        var botUserExists = await dbContext.Users.AnyAsync(u => u.Id == botUserId, cancellationToken: cancellationToken);
        if (!botUserExists)
        {
            var botUser = User.CreateBotUser(
                botUserId,
                ChatbotConstants.BotFirstName,
                ChatbotConstants.BotLastName);

            dbContext.Users.Add(botUser);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Seed Admin User if not exists
        var adminEmailResult = Fayora.Domain.ValueObjects.Email.Create(AdminConstants.AdminEmail);
        var adminExists = await dbContext.Users.AnyAsync(u => u.Id == AdminConstants.AdminId || (adminEmailResult.IsSuccess && u.PrimaryEmail == adminEmailResult.Value), cancellationToken: cancellationToken);

        if (!adminExists)
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var adminResult = User.CreateAdmin(
                AdminConstants.AdminId,
                AdminConstants.AdminFirstName,
                AdminConstants.AdminLastName,
                AdminConstants.AdminEmail,
                AdminConstants.AdminPassword,
                AdminConstants.ProfileImageUrl,
                passwordHasher);

            if (adminResult.IsSuccess)
            {
                dbContext.Users.Add(adminResult.Value);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
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

        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<CleanupDiscountOffersJob>(
                "cleanup-expired-cancelled-offers",
                job => job.ExecuteAsync(CancellationToken.None),
                Cron.Daily());

        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ProcessAutomaticPayoutsJob>(
                "process-automatic-payouts",
                job => job.ExecuteAsync(CancellationToken.None),
                Cron.Daily());

        return app;
    }
}