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

    public async Task<TourGuide?> GetGuideByIdAsync(Guid id, bool IsReadOnly, CancellationToken cancellationToken)
    {
        IQueryable<TourGuide> query = context.TourGuides;

        if (IsReadOnly)
        {
            query = query.AsNoTracking();
        }

        return await query
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}
