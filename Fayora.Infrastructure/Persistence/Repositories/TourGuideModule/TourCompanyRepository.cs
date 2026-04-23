using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Entities.TourGuideModule;
using Fayora.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class TourCompanyRepository(ApplicationDbContext context) : ITourCompanyRepository
{
    public async Task AddTourCompanyAsync(TourCompany tourCompany, CancellationToken cancellationToken = default)
    {
        await context.TourCompanies.AddAsync(tourCompany, cancellationToken);
    }

    public async Task<TourCompany?> GetTourCompanyByIdAsync(
        Guid id,
        ITourGuideRepository.GuideQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var query = context.TourCompanies.AsQueryable();

        if (options.ReadOnly)
            query = query.AsNoTracking();

        if (options.IncludeTourPackageIds && !options.ReadOnly)
            query = query.Include("Packages");

        return await query.FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);
    }

    public async Task<TourCompany?> GetTourCompanyByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.TourCompanies.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.TourCompanies.AnyAsync(x => x.UserId == userId, cancellationToken);
}