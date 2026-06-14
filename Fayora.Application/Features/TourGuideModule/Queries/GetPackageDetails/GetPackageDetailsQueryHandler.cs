using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Contracts.TourGuideModule.GetPackageDetails;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails
{
    public class GetPackageDetailsQueryHandler(
        IPackageRepository packageRepository,
        ITourGuideRepository tourGuideRepository,
        IPackageNightRepository packageNightRepository,
        IUserRepository userRepository,
        IDiscountOfferRepository discountOfferRepository,
        IPackageImageRepository packageImageRepository)
        : IQueryHandler<GetPackageDetailsQuery, Result<PackageDetailsResult>>
    {
        public async Task<Result<PackageDetailsResult>> Handle(
            GetPackageDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var package = await packageRepository.GetPackageWithOccurrencesAsync(
                request.PackageId, cancellationToken);
            if (package is null) return TourGuideErrors.PackageNotFound;

            var activities = await packageRepository.GetActivitiesByPackageIdAsync(
                request.PackageId, cancellationToken);

            var nights = await packageNightRepository.GetByPackageIdAsync(
                request.PackageId, cancellationToken);

            var meetingPoints = await packageRepository.GetMeetingPointsByPackageIdAsync(
                 request.PackageId, cancellationToken);

            var packageImages = await packageImageRepository.GetPackageImages(
                package.Id, cancellationToken);

            var guide = await tourGuideRepository.GetGuideByIdAsync(
                package.UserId,
                new GuideQueryOptions(ReadOnly: true),
                cancellationToken);
            if (guide is null) return TourGuideErrors.GuideNotFound;

            var user = await userRepository.GetUserByIdAsync(
                guide.UserId,
                new UserQueryOptions { IsReadOnly = true },
                cancellationToken);
            if (user is null) return AuthErrors.UserNotFound;

            decimal discountedAdultPrice = package.AdultPrice;
            decimal discountedChildPrice = package.ChildPrice;

            var activeOffers = await discountOfferRepository.GetActiveByTargetAsync(
                package.Id, OfferTargetType.GuidePackage, cancellationToken);

            var offer = activeOffers.FirstOrDefault();
            if (offer is not null)
            {
                var adultResult = offer.ApplyTo(package.AdultPrice);
                var childResult = offer.ApplyTo(package.ChildPrice);
                if (!adultResult.IsError && !childResult.IsError)
                {
                    discountedAdultPrice = adultResult.Value;
                    discountedChildPrice = childResult.Value;
                }
            }

            return new PackageDetailsResult(
                package.Title,
                package.Description,
                package.AdultPrice,
                package.ChildPrice,
                discountedAdultPrice,
                discountedChildPrice,
                package.DurationHours,
                package.NumOfDays,
                package.NumOfNights,
                package.MainImageUrl.Value,
                packageImages.Select(img => img.ImageUrl.Value).ToList(),
                package.IncludedItemIds.ToList(),
                package.ExcludedItemIds?.ToList(),
                activities.Select(a => new PackageActivityDetailsResult(
                    a.Description,
                    a.ActivityTime,
                    a.IsOptional,
                    a.LocationId,
                    a.Place?.Latitude,
                    a.Place?.Longitude)).ToList(),
                nights.Select(n => new PackageNightDetailsResult(
                    n.Id,
                    n.NightNumber,
                    n.NightDate,
                    n.HousingUnitId,
                    n.PackageAccommodationId)).ToList(),
                meetingPoints.Select(mp => new PackageMeetingPointResult(
                    mp.Id,
                    mp.MeetingPointName,
                    mp.MeetingPoint.Latitude,
                    mp.MeetingPoint.Longitude,
                    mp.Time,
                    mp.Price,
                    mp.Description)).ToList(),
                new GuideInfoDetailsResult(
                    guide.UserId,
                    user.FirstName,
                    user.LastName,
                    user.ProfileImageUrl?.Value,
                    guide.AverageRating,
                    guide.ReviewCount,
                    guide.CompletedToursCount),
                package.CancellationPolicy.ToString(),
                package.TransportType.ToString(),
                package.GuestRequirements,
                package.ArrivalNote,
                package.LocationIds.ToList(),
                package.Occurrences.Select(o => new PackageOccurrenceResult(
                    o.Id,
                    o.Date,
                    o.AvailableSeats)).ToList(),
                package.OptionalActivities.Select(a => new OptionalActivityResponse(
                    a.Id,
                    a.Description,
                    a.AdditionalPrice,
                    a.ImageUrl.Value)).ToList(),
                package.TourTypes.ToString(),
                package.MaxCapacity,
                package.MainVideoUrl?.Value);
        }
    }
}
