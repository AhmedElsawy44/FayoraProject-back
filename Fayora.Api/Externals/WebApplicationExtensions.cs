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

        await dbContext.Database.MigrateAsync();

        // Seed Chatbot User if not exists
        var botUserId = Fayora.Domain.Common.ChatbotConstants.BotUserId;
        var botUserExists = await dbContext.Users.AnyAsync(u => u.Id == botUserId);
        if (!botUserExists)
        {
            var botUser = Fayora.Domain.Entities.IdentityModule.User.CreateBotUser(
                botUserId,
                Fayora.Domain.Common.ChatbotConstants.BotFirstName,
                Fayora.Domain.Common.ChatbotConstants.BotLastName);

            dbContext.Users.Add(botUser);
            await dbContext.SaveChangesAsync();
        }

        // Seed Admin User if not exists or ensure Admin credentials/roles
        var adminEmailStr = "Fayoratravel@gmail.com";
        var adminEmailResult = Fayora.Domain.ValueObjects.Email.Create(adminEmailStr);
        if (adminEmailResult.IsSuccess)
        {
            var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.PrimaryEmail == adminEmailResult.Value);
            var passwordHasher = scope.ServiceProvider.GetRequiredService<Fayora.Domain.Common.Interfaces.IdentityModule.IPasswordHasher>();

            if (adminUser is null)
            {
                var adminResult = Fayora.Domain.Entities.IdentityModule.User.CreateWithEmail(
                    "Fayora",
                    "Admin",
                    adminEmailStr,
                    "Ahmed@Ahmed@123",
                    passwordHasher);

                if (adminResult.IsSuccess)
                {
                    adminUser = adminResult.Value;
                    adminUser.VerifyEmail();
                    adminUser.VerifyPhone();
                    adminUser.AddRole(Fayora.Domain.Enums.IdentityModule.Role.Admin);

                    dbContext.Users.Add(adminUser);
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                if (adminUser.Roles is null || !adminUser.Roles.Value.HasFlag(Fayora.Domain.Enums.IdentityModule.Role.Admin))
                {
                    adminUser.AddRole(Fayora.Domain.Enums.IdentityModule.Role.Admin);
                }

                if (!adminUser.IsEmailVerified)
                {
                    adminUser.VerifyEmail();
                }

                if (!adminUser.IsCorrectPasswordHash("Ahmed@Ahmed@123", passwordHasher))
                {
                    adminUser.ChangePassword("Ahmed@Ahmed@123", passwordHasher);
                    adminUser.AdminUpdateStatus(Fayora.Domain.Enums.IdentityModule.UserStatus.Active);
                }

                await dbContext.SaveChangesAsync();
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

        return app;
    }
}