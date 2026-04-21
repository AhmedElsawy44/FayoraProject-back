using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;

public class TourGuideRepository(ApplicationDbContext context) : ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide)
    {
        context.TourGuides.Add(tourGuide);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.TourGuides
            .AnyAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<TourGuide?> GetGuideByIdAsync(Guid id, ITourGuideRepository.GuideQueryOptions options, CancellationToken cancellationToken)
    {
        var query = context.TourGuides.AsQueryable();

        if (options is null)
        {
            return await query.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        if (options.ReadOnly)
        {
            query = query.AsNoTracking();
        }

        if (options.IncludeCities)
        {
            query = query.Include(g => g.GuideCities)
                         .ThenInclude(gc => gc.City);
        }

        if (options.IncludeTourPackages)
        {
            query = query.Include(g => g.TourPackages);
        }

        return await query.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}
