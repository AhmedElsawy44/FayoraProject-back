using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence;

internal class ApplicationDbContext() : DbContext(), IUnitOfWork
{
    public DbSet<User> users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<BannedItem> BannedItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserArchive> UserArchives { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }
    public DbSet<UserIdentity> UserIdentities { get; set; }
    public DbSet<VerificationCode> VerificationCode { get; set; }
    public DbSet<VerificationDocument> VerificationDocuments { get; set; }
    public DbSet<VerificationRequest> VerificationRequests { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }

    public async Task CommitChangesAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
