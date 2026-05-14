using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails
{
    public class GetPackageDetailsQueryHandler(
        IPackageRepository packageRepository,
        ITourGuideRepository tourGuideRepository,
        IUserRepository userRepository)
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

            return new PackageDetailsResult(
                package.Title,
                package.Description,
                package.AdultPrice,
                package.ChildPrice,
                package.DurationHours,
                package.MainImageUrl.Value,
                package.ImageIds.Select(id => id.ToString()).ToList(),
                package.IncludedItemIds.ToList(),
                package.ExcludedItemIds?.ToList(),
                activities.Select(a => new PackageActivityDetailsResult(
                    a.Description,
                    a.ActivityTime,
                    a.IsOptional)).ToList(),
                new GeoPointDetailsResult(
                    package.MeetingPoint.Latitude,
                    package.MeetingPoint.Longitude),
                new GuideInfoDetailsResult(
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
                package.Occurrences.Select(o => new PackageOccurrenceResult(
                    o.Id,
                    o.Date,
                    o.AvailableSeats)).ToList());
        }
    }
}
