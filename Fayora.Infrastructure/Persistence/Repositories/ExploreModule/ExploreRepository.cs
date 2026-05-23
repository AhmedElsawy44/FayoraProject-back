using Fayora.Application.Common.Interfaces.Persistences.ExploreModule;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Infrastructure.Persistence.Repositories.ExploreModule;

public class ExploreRepository(ApplicationDbContext context) : IExploreRepository
{
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
