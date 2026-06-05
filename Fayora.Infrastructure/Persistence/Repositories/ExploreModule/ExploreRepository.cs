using Fayora.Application.Common.Interfaces.Persistences.ExploreModule;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.Enums.TouristModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.ExploreModule;

public class ExploreRepository(ApplicationDbContext context) : IExploreRepository
{
    public async Task<List<ExploreItemDto>> GetExploreItemsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        // Clamp parameters
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int skip = (pageNumber - 1) * pageSize;
        int take = pageSize;

        // Fetch User's favorite entity IDs
        var favoriteIds = new HashSet<Guid>();
        if (userId != Guid.Empty)
        {
            favoriteIds = await context.UserInteractions
                .AsNoTracking()
                .Where(ui => ui.UserId == userId && ui.InteractionType == InteractionType.Favorite)
                .Select(ui => ui.EntityId)
                .ToHashSetAsync(cancellationToken);
        }

        // 1. Fetch Locations
        var dbLocations = await context.Locations
            .AsNoTracking()
            .OrderByDescending(l => l.Rating)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 2. Fetch Active Packages
        var dbPackages = await context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.PackageStatus == ItemStatus.Active && p.DeletedAt == null)
            .OrderByDescending(p => p.Views)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 3. Fetch Active Housing Units
        var dbHousing = await context.HousingUnits
            .AsNoTracking()
            .Where(h => h.Status == ItemStatus.Active)
            .OrderByDescending(h => h.Rating)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 4. Fetch Active Tour Guides with User profile details and covered cities
        var dbGuides = await context.TourGuides
            .AsNoTracking()
            .Include(g => g.GuideCities)
                .ThenInclude(gc => gc.City)
            .Where(g => g.Status == ItemStatus.Active)
            .Join(context.Users.AsNoTracking(),
                g => g.UserId,
                u => u.Id,
                (g, u) => new { Guide = g, User = u })
            .OrderByDescending(gu => gu.Guide.AverageRating)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 5. Map to Explore DTOs
        var places = dbLocations.Select(l => new ExploreItemDto(
            Id: $"place_{l.Id}",
            Type: "place",
            Title: l.Name,
            ImageUrl: l.MainImageUrl.Value,
            IsFavorite: false,
            CardHeight: 290.0,
            Rating: (double)l.Rating,
            BadgeLabel: l.Category.ToString()
        )).ToList();

        var inspirations = dbLocations.Select(l => new ExploreItemDto(
            Id: $"inspiration_{l.Id}",
            Type: "inspiration",
            Title: l.Name,
            ImageUrl: l.MainImageUrl.Value,
            IsFavorite: false,
            CardHeight: 298.0,
            Rating: (double)l.Rating,
            BadgeLabel: "MUST SEE"
        )).ToList();

        var trips = dbPackages.Select(p => new ExploreItemDto(
            Id: $"trip_{p.Id}",
            Type: "trip",
            Title: p.Title,
            ImageUrl: p.MainImageUrl.Value,
            IsFavorite: favoriteIds.Contains(p.Id),
            CardHeight: 255.0,
            DurationLabel: $"{p.DurationHours}h",
            PriceLabel: $"EGP {p.AdultPrice}"
        )).ToList();

        var hotels = dbHousing.Select(h => new ExploreItemDto(
            Id: $"hotel_{h.Id}",
            Type: "hotel",
            Title: h.Title,
            ImageUrl: h.MainImageUrl.Value,
            IsFavorite: favoriteIds.Contains(h.Id),
            CardHeight: 278.0,
            LocationLabel: h.AddressDetails ?? "Fayoum",
            PriceLabel: $"EGP {h.PricePerNight}",
            Rating: (double)h.Rating
        )).ToList();

        var guides = dbGuides.Select(gu => {
            var cityNames = gu.Guide.GuideCities
                .Select(gc => gc.City?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
            var locationLabel = cityNames.Count > 0 
                ? string.Join(", ", cityNames) 
                : "Local Guide";

            var baseRateLabel = gu.Guide.BaseRate.HasValue 
                ? $"EGP {gu.Guide.BaseRate.Value}/h" 
                : "Negotiable";

            return new ExploreItemDto(
                Id: $"guide_{gu.Guide.UserId}",
                Type: "guide",
                Title: $"{gu.User.FirstName} {gu.User.LastName}".Trim(),
                ImageUrl: gu.User.ProfileImageUrl?.Value ?? "",
                IsFavorite: favoriteIds.Contains(gu.Guide.UserId),
                CardHeight: 262.0,
                LocationLabel: locationLabel,
                PriceLabel: baseRateLabel,
                Rating: (double)gu.Guide.AverageRating
            );
        }).ToList();

        // Interleave the items round-robin up to pageSize
        var result = new List<ExploreItemDto>();
        int index = 0;
        int maxIndex = Math.Max(
            Math.Max(places.Count, inspirations.Count),
            Math.Max(trips.Count, Math.Max(hotels.Count, guides.Count))
        );

        while (result.Count < pageSize && index < maxIndex)
        {
            if (index < places.Count)
            {
                result.Add(places[index]);
                if (result.Count >= pageSize) break;
            }
            if (index < inspirations.Count)
            {
                result.Add(inspirations[index]);
                if (result.Count >= pageSize) break;
            }
            if (index < trips.Count)
            {
                result.Add(trips[index]);
                if (result.Count >= pageSize) break;
            }
            if (index < hotels.Count)
            {
                result.Add(hotels[index]);
                if (result.Count >= pageSize) break;
            }
            if (index < guides.Count)
            {
                result.Add(guides[index]);
                if (result.Count >= pageSize) break;
            }
            index++;
        }

        return result;
    }
}
