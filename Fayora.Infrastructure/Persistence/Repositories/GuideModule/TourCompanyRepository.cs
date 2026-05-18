using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class TourCompanyRepository(ApplicationDbContext context) : ITourCompanyRepository
{
    public void AddTourCompany(TourCompany tourCompany)
    {
        context.TourCompanies.Add(tourCompany);
    }

    public async Task<TourCompany?> GetTourCompanyByIdAsync(
        Guid id,
        ITourGuideRepository.GuideQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var query = context.TourCompanies.AsQueryable();

        if (options.ReadOnly)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);
    }

    public async Task<int> GetActiveCompaniesCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.TourCompanies
            .CountAsync(x => x.IsAvailableForBooking, cancellationToken);
    }

    public async Task<int> GetCompaniesOnboardingStatsAsync(CancellationToken cancellationToken = default)
    {
        int totalInOnboarding = await context.TourCompanies
            .CountAsync(x => x.Status == ItemStatus.Pending, cancellationToken);

        return totalInOnboarding;
    }

    public Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.TourCompanies.AnyAsync(x => x.UserId == userId, cancellationToken);
    }
}