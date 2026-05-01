using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AdminModule;

public class InventoryModerationService(ApplicationDbContext context) : IInventoryModerationService
{
    public async Task<List<InventoryQueueItemDto>> GetInventoryQueueAsync(TypeFilter? typeFilter, int page, int pageSize, CancellationToken ct)
    {
        var accommodations = from h in context.HousingUnits
                             join u in context.Users on h.OwnerId equals u.Id
                             where h.Status == ItemStatus.Pending
                             select new
                             {
                                 h.Id,
                                 h.Title,
                                 Type = "Accommodations",
                                 FullName = u.FirstName + " " + u.LastName,
                                 Price = h.PricePerNight,
                                 Unit = "/night",
                                 ImageUrl = h.MainImageUrl.Value
                             };

        var trips = from t in context.GuideTourPackages
                    join u in context.Users on t.UserId equals u.Id
                    where t.Status == ItemStatus.Pending
                    select new
                    {
                        t.Id,
                        t.Title,
                        Type = "Trips",
                        FullName = u.FirstName + " " + u.LastName,
                        Price = t.AdultPrice,
                        Unit = "/seat",
                        ImageUrl = (string)t.MainImageUrl.Value
                    };

        var combinedQuery = accommodations.Concat(trips);

        if (typeFilter is not null && typeFilter != TypeFilter.All)
        {
            var typeString = typeFilter.ToString();
            combinedQuery = combinedQuery.Where(x => x.Type == typeString);
        }

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var pagedData = await combinedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return [.. pagedData.Select(x => new InventoryQueueItemDto(
            x.Id,
            x.Title,
            x.Type,
            x.FullName,
            x.Price,
            x.Unit,
            x.ImageUrl
        ))];
    }
}