using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Domain.Entities.TouristModule;
using Microsoft.EntityFrameworkCore;
using Fayora.Contracts.AdminModule.MasterInterests;

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

    public async Task<List<GetMasterInterestsResponse>> GetMasterInterestsAsync(CancellationToken ct)
    {
        var interests = await context.MasterInterests
            .OrderBy(i => i.SortOrder)
            .ToListAsync(ct);

        return interests.Select(i => new GetMasterInterestsResponse(
            i.Id, i.Code, i.Name, i.IconUrl, i.SortOrder, i.IsActive, i.CreateAt
        )).ToList();
    }

    public async Task<MasterInterest?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.MasterInterests.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public void Add(MasterInterest interest)
    {
        context.MasterInterests.Add(interest);
    }

    public void Remove(MasterInterest interest)
    {
        context.MasterInterests.Remove(interest);
    }
}

