using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Features.TourGuideModule.Common;
using static Fayora.Application.Common.Interfaces.Persistences.TourGuideModule.ITourGuideRepository;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById;

public class GetTourGuideByIdQueryHandler(
    ITourGuideRepository tourGuideRepository) : IRequestHandler<GetTourGuideByIdQuery, Result<GetTourGuideByIdResult>>
{
    public async Task<Result<GetTourGuideByIdResult>> Handle(GetTourGuideByIdQuery request, CancellationToken cancellationToken)
    {
        var tourGuide = await tourGuideRepository.GetGuideByIdAsync(request.TourGuideId, new GuideQueryOptions { IncludeCities = true, IncludeTourPackages = true }, cancellationToken);

        if (tourGuide is null) return TourGuideErrors.GuideNotFound;

        return new GetTourGuideByIdResult(
            tourGuide.Id,
            tourGuide.UserId,
            tourGuide.BaseRate,
            tourGuide.YearsOfExperience,
            tourGuide.LicenseNumber,
            tourGuide.LicenseExpiryDate,
            tourGuide.TaxRegistrationNumber,
            tourGuide.TaxRegistrationDate,
            tourGuide.CurrencyCode,
            tourGuide.ReviewCount,
            tourGuide.AverageRating,
            tourGuide.Status,
            tourGuide.IsAvailableForBooking,
            tourGuide.IsOnline,
            tourGuide.CompletedToursCount,
            tourGuide.IsSuperGuide,
            tourGuide.CancellationRate,
            tourGuide.TransportInfo,
            tourGuide.ResponseRate,
            [.. tourGuide.GuideCities.Select(c => c.City)],
            [.. tourGuide.TourPackages.Select(p => new GuideTourPackageSummaryDto(
                p.Id,
                p.Title,
                p.MainImageUrl,
                p.AdultPrice,
                p.DurationHours,
                p.TourTypes,
                p.AvailableSpots
            ))]
        );
    }
}