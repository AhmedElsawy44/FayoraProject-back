using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Entitties.Tourist;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

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
    public DbSet<MasterInterest> MasterInterests { get; set; }
    public DbSet<TouristProfile> Tourists { get; set; }


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
}
