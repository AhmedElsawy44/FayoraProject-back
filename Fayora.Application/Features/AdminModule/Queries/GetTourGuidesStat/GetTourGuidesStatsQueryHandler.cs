using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourGuidesStat;

public class GetTourGuidesStatsHandler(
    ITourGuideRepository guideRepository,
    IBookingRepository bookingRepository) : IQueryHandler<GetTourGuidesStatsQuery, TourGuidesStatsResponse>
{
    public async Task<TourGuidesStatsResponse> Handle(
        GetTourGuidesStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await guideRepository.GetTourGuidesStatsAsync(cancellationToken);

        var onTourNow = await bookingRepository.GetOnTourNowCountAsync(cancellationToken);

        var guideRevenue = await bookingRepository.GetGuideRevenueAsync(cancellationToken);

        return new TourGuidesStatsResponse(stats.activeGuide, stats.avgRating, onTourNow, guideRevenue);
    }
}
