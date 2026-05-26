using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Features.AdminModule.Queries.GetTourGuideVerificationDetails;
using Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;
using Fayora.Contracts.AdminModule.GetVerificationQueue;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

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
    public async Task<List<GetVerificationQueueResoponse>> GetPendingGuidesForVerificationAsync(CancellationToken cancellationToken)
    {
        return await (from guide in context.TourGuides
                      join user in context.Users on guide.UserId equals user.Id
                      where guide.Status == ItemStatus.Pending
                      orderby guide.CreatedAt descending
                      select new GetVerificationQueueResoponse
                      (
                          guide.UserId,
                          user.FullName,
                          PartnerTypeFilter.Guide,
                          1,
                          guide.CreatedAt.UtcDateTime
                      ))
                      .ToListAsync(cancellationToken);
    }

    public async Task<(int activeGuide, decimal avgRating)> GetTourGuidesStatsAsync(CancellationToken cancellationToken)
    {
        int totalActiveGuides = await context.TourGuides
            .CountAsync(t => t.IsAvailableForBooking == true, cancellationToken);

        decimal? avgRatingNullable = await context.TourGuides
            .AverageAsync(t => (decimal?)t.AverageRating, cancellationToken);

        decimal currentAvgRating = avgRatingNullable ?? 0.0m;

        currentAvgRating = Math.Round(currentAvgRating, 2);

        return (totalActiveGuides, currentAvgRating);
    }

    public async Task<GetTourGuideVerificationDetailsResponse?> GetVerificationDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var guideWithUser = await (from guide in context.TourGuides
                                   join user in context.Users on guide.UserId equals user.Id
                                   where guide.UserId == id
                                   select new { guide, user })
                                   .FirstOrDefaultAsync(cancellationToken);

        if (guideWithUser is null)
        {
            return null;
        }

        var currentGuide = guideWithUser.guide;
        var currentUser = guideWithUser.user;
        var cityNames = await context.Cities
            .Where(c => currentGuide.GuideCities.Select(gc => gc.CityId).Contains(c.Id))
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);

        var transportFeatures = new List<string>();
        if (currentGuide.TransportInfo.HasValue)
        {
            var flags = currentGuide.TransportInfo.Value;
            if (flags.HasFlag(TransportInfo.HasOwnVehicle)) transportFeatures.Add("Has Own Vehicle");
            if (flags.HasFlag(TransportInfo.VehicleDetails)) transportFeatures.Add("Vehicle Details Provided");
            if (flags.HasFlag(TransportInfo.TransportType)) transportFeatures.Add("Transport Type Specified");
        }

        return new GetTourGuideVerificationDetailsResponse
        (
            currentGuide.UserId,
            currentUser.FullName,
            currentUser.PrimaryEmail?.Value,
            currentUser.PhoneNumber?.Value,
            currentGuide.LicenseNumber,
            currentGuide.ProfessionalLicenseUrl?.Value,
            currentGuide.LicenseExpiryDate,
            currentGuide.YearsOfExperience,
            currentGuide.BaseRate,
            currentGuide.PricingUnit?.ToString(),
            transportFeatures,
            cityNames
        );
    }

    public async Task<bool> TourGuideExistAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync(id, cancellationToken);
    }
}