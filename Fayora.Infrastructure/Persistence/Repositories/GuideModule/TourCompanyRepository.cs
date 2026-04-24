using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;

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

    public async Task<TourCompany?> GetTourCompanyByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.TourCompanies.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.TourCompanies.AnyAsync(x => x.UserId == userId, cancellationToken);
}