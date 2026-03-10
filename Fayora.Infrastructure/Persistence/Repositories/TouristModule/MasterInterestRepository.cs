using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using Fayora.Domain.Entitties.Tourist;
using Fayora.Infrastructure.Persistence.Repositories.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TouristModule;

public class MasterInterestRepository(ApplicationDbContext context) : IMasterInterestRepository
{
    public async Task<IEnumerable<MasterInterest>> GetAllInterestsAsync()
        => await context.MasterInterests
            .AsNoTracking()
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
}
