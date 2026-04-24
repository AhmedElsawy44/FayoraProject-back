using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
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
            .AnyAsync(g => g.UserId == id, cancellationToken);
    }

    public async Task<TourGuide?> GetGuideByIdAsync(
    Guid id,
    ITourGuideRepository.GuideQueryOptions options,
    CancellationToken cancellationToken)
    {
        IQueryable<TourGuide> query = context.TourGuides;

        if (options.ReadOnly)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(g => g.UserId == id, cancellationToken);
    }

    public async Task<bool> TourGuideExistAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync(id, cancellationToken);
    }
}