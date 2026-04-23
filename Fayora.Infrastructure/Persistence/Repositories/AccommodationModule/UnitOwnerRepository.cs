using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class UnitOwnerRepository(ApplicationDbContext context) : IUnitOwnerRepository
{
    public void AddOwner(UnitOwner owner)
    {
        context.Add(owner);
    }

    public Task<UnitOwner?> GetOwnerByUserIdAsync(Guid userId, bool isReadOnly = true, CancellationToken cancellationToken = default)
    {
        if (isReadOnly)
        {
            return context.UnitOwners.AsNoTracking().FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
        }
        else
        {
            return context.UnitOwners.FindAsync(userId, cancellationToken).AsTask();
        }
    }

    public async Task<bool> UnitOwnerExistAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UnitOwners
        .AnyAsync(o => o.UserId == userId, cancellationToken);
    }
}
