//using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
//using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
//using Fayora.Application.Features.TourGuideModule.Common;
//using static Fayora.Application.Common.Interfaces.Persistences.TourGuideModule.ITourGuideRepository;
//using Fayora.Domain.Common.Results;
//using Fayora.Domain.Enums.SharedModule;
//using MediatR;

//namespace Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById;

//public class GetTourGuideByIdQueryHandler(
//    ITourGuideRepository tourGuideRepository,
//    IDiscountOfferRepository discountOfferRepository) : IRequestHandler<GetTourGuideByIdQuery, Result<GetTourGuideByIdResult>>
//{
//    public async Task<Result<GetTourGuideByIdResult>> Handle(GetTourGuideByIdQuery request, CancellationToken cancellationToken)
//    {
//        var tourGuide = await tourGuideRepository.GetGuideByIdAsync(request.TourGuideId, new GuideQueryOptions { IncludeCities = true, IncludeTourPackageIds = true }, cancellationToken);

//        if (tourGuide is null) return TourGuideErrors.GuideNotFound;

//        decimal originalBaseRate = tourGuide.BaseRate ?? 0;
//        decimal discountedBaseRate = originalBaseRate;
//        if (tourGuide.BaseRate.HasValue)
//        {
//            var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
//                tourGuide.UserId, OfferTargetType.TourGuide, cancellationToken);
//            var offer = activeOffers.FirstOrDefault();
//            if (offer is not null)
//            {
//                var discountResult = offer.ApplyTo(originalBaseRate);
//                if (!discountResult.IsError)
//                {
//                    discountedBaseRate = discountResult.Value;
//                }
//            }
//        }

//        return new GetTourGuideByIdResult(
//            tourGuide.UserId,
//            originalBaseRate,
//            discountedBaseRate,
//            tourGuide.YearsOfExperience,
//            tourGuide.LicenseNumber,
//            tourGuide.LicenseExpiryDate,
//            tourGuide.CurrencyCode,
//            tourGuide.ReviewCount,
//            tourGuide.AverageRating,
//            tourGuide.Status,
//            tourGuide.IsAvailableForBooking,
//            tourGuide.IsOnline,
//            tourGuide.CompletedToursCount,
//            tourGuide.IsSuperGuide,
//            tourGuide.CancellationRate,
//            tourGuide.TransportInfo,
//            tourGuide.ResponseRate,
//            [.. tourGuide.GuideCities.Select(c => c.City)],
//            [.. tourGuide.TourPackages.Select(p => new GuideTourPackageSummaryDto(
//                p.Id,
//                p.Title,
//                p.MainImageUrl,
//                p.AdultPrice,
//                p.DurationHours,
//                p.TourTypes,
//                p.AvailableSpots
//            ))]
//        );
// 