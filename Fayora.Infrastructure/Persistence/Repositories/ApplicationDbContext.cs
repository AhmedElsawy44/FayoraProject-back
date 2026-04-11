using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AIModule;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor, IPublisher publisher) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserIdentity> UserIdentities { get; set; }
    public DbSet<UserTokens> UserTokens { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }
    public DbSet<VerificationDocument> VerificationDocuments { get; set; }
    public DbSet<VerificationRequest> VerificationRequests { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<HasDomainEvents>()
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(publisher, domainEvents);
        }

        await SaveChangesAsync(cancellationToken);
    }

    private static async Task PublishDomainEvents(IPublisher publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }

    private bool IsUserWaitingOnline() => httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        var domainEventsQueue = httpContextAccessor.HttpContext!.Items
            .TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new Queue<IDomainEvent>();

        domainEvents.ForEach(domainEventsQueue.Enqueue);

        httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }


    public async Task<IEnumerable<object>> SearchAsync(string intent, IAIService.SearchParams parameters)
    {
        return intent?.ToLower() switch
        {
            "housing" => await SearchHotelsAsync(parameters),
            "trip" => await SearchTripsAsync(parameters),
            "guiding" => await SearchGuidesAsync(parameters),
            _ => Enumerable.Empty<object>()
        };
    }

    private async Task<IEnumerable<object>> SearchHotelsAsync(IAIService.SearchParams parameters)
    {
        var query = this.Set<Hotel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Location))
            query = query.Where(h => h.City.Contains(parameters.Location) || h.Name.Contains(parameters.Location));

        if (parameters.Budget_Max.HasValue)
            query = query.Where(h => h.PricePerNight <= parameters.Budget_Max.Value);

        if (parameters.People_Count.HasValue)
            query = query.Where(h => h.MaxCapacity >= parameters.People_Count.Value);

        return await query.AsNoTracking().Take(10).ToListAsync();
    }

    private async Task<IEnumerable<object>> SearchTripsAsync(IAIService.SearchParams parameters)
    {
        var query = this.Set<Trip>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Location))
            query = query.Where(t => t.Destination.Contains(parameters.Location));

        if (parameters.Budget_Max.HasValue)
            query = query.Where(t => t.Price <= parameters.Budget_Max.Value);

        if (parameters.Duration_Days.HasValue)
            query = query.Where(t => t.DurationInDays == parameters.Duration_Days.Value);

        return await query.AsNoTracking().Take(10).ToListAsync();
    }

    private async Task<IEnumerable<object>> SearchGuidesAsync(IAIService.SearchParams parameters)
    {
        var query = this.Set<Guide>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Location))
            query = query.Where(g => g.CitiesCovered.Contains(parameters.Location));

        if (parameters.Budget_Max.HasValue)
            query = query.Where(g => g.DailyRate <= parameters.Budget_Max.Value);

        return await query.AsNoTracking().Take(10).ToListAsync();
    }
}
