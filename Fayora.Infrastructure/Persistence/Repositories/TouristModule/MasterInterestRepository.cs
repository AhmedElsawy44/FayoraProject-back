using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using Fayora.Domain.Entities.TouristModule;
using Fayora.Infrastructure.Persistence.Repositories.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TouristModule;

public class MasterInterestRepository(ApplicationDbContext context) : IMasterInterestRepository
{
    public async Task<IEnumerable<MasterInterest>> GetAllInterestsAsync()
        => await context.MasterInterests
            .AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

    public async Task<bool> InterestsExistAsync(IEnumerable<int> interestIds, CancellationToken cancellationToken)
    {
        var uniqueIds = interestIds.Distinct().ToList();

        if (!uniqueIds.Any())
        {
            return true; 
        }

        var existingCount = await context.MasterInterests
            .CountAsync(i => uniqueIds.Contains(i.Id), cancellationToken);

        return existingCount == uniqueIds.Count;
    }
}
