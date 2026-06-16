using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AdminModule.Queries.GetDetailedPackage;

public class GetDetailedPackageQueryHandler(
    IPackageRepository packageRepository,
    IPackageImageRepository packageImageRepository,
    IUserRepository userRepository


    ) : IQueryHandler<GetDetailedPackageQuery, Result<GetDetailedPackageResult>>
{
    public async Task<Result<GetDetailedPackageResult>> Handle(GetDetailedPackageQuery request, CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetPackageByIdAsync(request.PackageId, new PackageQueryOptions(), cancellationToken);

        if (package is null)
        {
            return TourGuideErrors.PackageNotFound;
        }

        var user = await userRepository.GetUserByIdAsync(package.UserId, new UserQueryOptions(), cancellationToken);

        if (user is null)
        {
            return TourGuideErrors.GuideNotFound;
        }

        var imageUrls = await packageImageRepository.GetPackageImages(request.PackageId, cancellationToken);

        var meetingPoints = await packageRepository.GetMeetingPointsByPackageIdAsync(package.Id, cancellationToken);

        return new GetDetailedPackageResult
        (
            package.Id,
            package.UserId,
            user.FullName,
            package.Title,
            package.Description,
            package.TourTypes.ToString(),
            package.DurationHours,
            package.MaxCapacity,
            package.AdultPrice,
            package.ChildPrice,
            package.MainImageUrl.Value,
            package.MainVideoUrl?.Value,
            package.GuestRequirements,
            package.CancellationPolicy.ToString(),
            package.IncludedItemIds.ToList(),
            package.ExcludedItemIds.ToList(),
            meetingPoints.Select(mp => new PackageMeetingPointResult(
                mp.Id,
                mp.MeetingPointName,
                mp.MeetingPoint.Latitude,
                mp.MeetingPoint.Longitude,
                mp.Time,
                mp.Price,
                mp.Description)).ToList(),
            package.ArrivalNote,
            package.TransportType.ToString(),
            imageUrls.Select(x => x.ImageUrl.Value).ToList()
        );
    }
}
