using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryStats;

public class GetInventoryStatsQueryHandler(
    IHousingUnitRepository housingUnitRepository,
    IBookingRepository bookingRepository
) : IQueryHandler<GetInventoryStatsQuery, InventoryStatsResponse>
{
    public async Task<InventoryStatsResponse> Handle(GetInventoryStatsQuery request, CancellationToken cancellationToken)
    {
        var liveListings = await housingUnitRepository.GetLiveListingsStatsAsync(cancellationToken);
        var pendingReview = await housingUnitRepository.GetPendingReviewStatsAsync(cancellationToken);

        var avgOccupancy = await bookingRepository.GetLocationsAvgOccupancyAsync(cancellationToken);

        return new InventoryStatsResponse(
            liveListings,
            pendingReview,
            avgOccupancy,
            0
        );
    }
}
