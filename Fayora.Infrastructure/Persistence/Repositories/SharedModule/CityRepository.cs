using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule;

public class CityRepository(ApplicationDbContext context) : ICityRepository
{
    public async Task<bool> CitiesExistsAsync(List<int> cityIds, CancellationToken cancellationToken)
    {
        if (cityIds.Count == 0)
            return true;

        var distinctCityIds = cityIds.Distinct().ToList();

        var existingCitiesCount = await context.Cities
            .Where(c => distinctCityIds.Contains(c.Id))
            .CountAsync(cancellationToken);

        return existingCitiesCount == distinctCityIds.Count;
    }
}
