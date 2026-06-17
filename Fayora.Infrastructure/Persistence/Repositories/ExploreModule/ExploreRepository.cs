using Fayora.Application.Common.Interfaces.Persistences.ExploreModule;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Enums.SharedModule;
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
        string? search,
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

        var cleanSearch = search?.Trim().ToLower();

        // 1. Fetch Locations
        var locationsQuery = context.Locations.AsNoTracking();
        if (!string.IsNullOrEmpty(cleanSearch))
        {
            locationsQuery = locationsQuery.Where(l => l.Name.ToLower().Contains(cleanSearch) || (l.Description != null && l.Description.ToLower().Contains(cleanSearch)));
        }
        var dbLocations = await locationsQuery
            .OrderByDescending(l => l.Rating)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 2. Fetch Active Packages
        var packagesQuery = context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.PackageStatus == ItemStatus.Active && p.DeletedAt == null);
        if (!string.IsNullOrEmpty(cleanSearch))
        {
            packagesQuery = packagesQuery.Where(p => p.Title.ToLower().Contains(cleanSearch) || (p.Description != null && p.Description.ToLower().Contains(cleanSearch)));
        }
        var dbPackages = await packagesQuery
            .OrderByDescending(p => p.Views)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 3. Fetch Active Housing Units
        var housingQuery = context.HousingUnits
            .AsNoTracking()
            .Where(h => h.Status == ItemStatus.Active);
        if (!string.IsNullOrEmpty(cleanSearch))
        {
            housingQuery = housingQuery.Where(h => h.Title.ToLower().Contains(cleanSearch) || (h.Description != null && h.Description.ToLower().Contains(cleanSearch)) || (h.AddressDetails != null && h.AddressDetails.ToLower().Contains(cleanSearch)));
        }
        var dbHousing = await housingQuery
            .OrderByDescending(h => h.Rating)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // 4. Fetch Active Tour Guides with User profile details and covered cities
        var guidesQuery = context.TourGuides
            .AsNoTracking()
            .Include(g => g.GuideCities)
                .ThenInclude(gc => gc.City)
            .Where(g => g.Status == ItemStatus.Active)
            .Join(context.Users.AsNoTracking(),
                g => g.UserId,
                u => u.Id,
                (g, u) => new { Guide = g, User = u });
        if (!string.IsNullOrEmpty(cleanSearch))
        {
            guidesQuery = guidesQuery.Where(gu => gu.User.FirstName.ToLower().Contains(cleanSearch) || gu.User.LastName.ToLower().Contains(cleanSearch));
        }
        var dbGuides = await guidesQuery
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

    public async Task<List<ExploreItemResponse>> GetExploreItemsAsync(
        string? search,
        string? type,
        List<Guid> wishlistIds,
        CancellationToken cancellationToken)
    {
        var itemsPool = new List<ExploreItemResponse>();
        var cleanSearch = search?.Trim().ToLower();

        // 1. Fetch Locations (Places/Attractions)
        if (string.IsNullOrEmpty(type) || type.Equals("all", StringComparison.OrdinalIgnoreCase) || type.Equals("location", StringComparison.OrdinalIgnoreCase))
        {
            var locationsQuery = context.Locations.AsNoTracking();

            if (!string.IsNullOrEmpty(cleanSearch))
            {
                locationsQuery = locationsQuery.Where(l => 
                    l.Name.ToLower().Contains(cleanSearch) || 
                    (l.Description != null && l.Description.ToLower().Contains(cleanSearch)));
            }

            var locations = await locationsQuery.ToListAsync(cancellationToken);

            itemsPool.AddRange(locations.Select(l => new ExploreItemResponse(
                l.Id.ToString(),
                l.Name,
                "location",
                l.MainImageUrl?.Value ?? "",
                l.Rating,
                l.ReviewCount,
                l.Category.ToString(),
                null,
                null,
                l.Rating >= 4.8m ? "MUST SEE" : (l.Category == LocationCategory.CulturalHub ? "INSPIRATION" : null),
                false
            )));
        }

        // 2. Fetch Packages (Tours/Trips)
        if (string.IsNullOrEmpty(type) || type.Equals("all", StringComparison.OrdinalIgnoreCase) || type.Equals("package", StringComparison.OrdinalIgnoreCase))
        {
            var packagesQuery = context.GuideTourPackages
                .AsNoTracking()
                .Where(p => p.IsActive && p.PackageStatus == ItemStatus.Active);

            if (!string.IsNullOrEmpty(cleanSearch))
            {
                packagesQuery = packagesQuery.Where(p => 
                    p.Title.ToLower().Contains(cleanSearch) || 
                    p.Description.ToLower().Contains(cleanSearch));
            }

            var packages = await packagesQuery.ToListAsync(cancellationToken);

            itemsPool.AddRange(packages.Select(p => new ExploreItemResponse(
                p.Id.ToString(),
                p.Title,
                "package",
                p.MainImageUrl?.Value ?? "",
                4.7m,
                p.Views > 0 ? p.Views / 2 + 3 : 5,
                "رحلة سياحية",
                $"EGP {p.AdultPrice:N0}",
                $"{p.DurationHours}h",
                p.AdultPrice < 600 ? "عرض خاص" : null,
                wishlistIds.Contains(p.Id)
            )));
        }

        // 3. Fetch Accommodations (Stays)
        if (string.IsNullOrEmpty(type) || type.Equals("all", StringComparison.OrdinalIgnoreCase) || type.Equals("accommodation", StringComparison.OrdinalIgnoreCase))
        {
            var accommodationsQuery = context.HousingUnits
                .AsNoTracking()
                .Where(h => h.Status == ItemStatus.Active);

            if (!string.IsNullOrEmpty(cleanSearch))
            {
                accommodationsQuery = accommodationsQuery.Where(h => 
                    h.Title.ToLower().Contains(cleanSearch) || 
                    (h.Description != null && h.Description.ToLower().Contains(cleanSearch)) || 
                    h.AddressDetails.ToLower().Contains(cleanSearch));
            }

            var accommodations = await accommodationsQuery.ToListAsync(cancellationToken);

            itemsPool.AddRange(accommodations.Select(h => new ExploreItemResponse(
                h.Id.ToString(),
                h.Title,
                "accommodation",
                h.MainImageUrl?.Value ?? "",
                h.Rating,
                h.ReviewCount,
                h.AddressDetails,
                $"EGP {h.PricePerNight:N0}/night",
                null,
                h.Rating >= 4.8m ? "رائج" : null,
                wishlistIds.Contains(h.Id)
            )));
        }

        // 4. Fetch TourGuides (Guides)
        if (string.IsNullOrEmpty(type) || type.Equals("all", StringComparison.OrdinalIgnoreCase) || type.Equals("guide", StringComparison.OrdinalIgnoreCase))
        {
            var guidesQuery = context.TourGuides
                .AsNoTracking()
                .Where(g => g.Status == ItemStatus.Active);

            var guidesList = await (from guide in guidesQuery
                                   join user in context.Users.AsNoTracking() on guide.UserId equals user.Id
                                   select new { guide, user })
                                   .ToListAsync(cancellationToken);

            if (!string.IsNullOrEmpty(cleanSearch))
            {
                guidesList = guidesList.Where(x => 
                    x.user.FirstName.ToLower().Contains(cleanSearch) || 
                    x.user.LastName.ToLower().Contains(cleanSearch))
                    .ToList();
            }

            itemsPool.AddRange(guidesList.Select(x => new ExploreItemResponse(
                x.guide.UserId.ToString(),
                x.user.FullName,
                "guide",
                x.user.ProfileImageUrl?.Value ?? "",
                x.guide.AverageRating,
                x.guide.ReviewCount,
                "مرشد سياحي مرخص",
                x.guide.BaseRate.HasValue ? $"EGP {x.guide.BaseRate.Value:N0}/day" : null,
                null,
                x.guide.IsSuperGuide ? "SUPER GUIDE" : null,
                wishlistIds.Contains(x.guide.UserId)
            )));
        }

        return itemsPool;
    }
}
