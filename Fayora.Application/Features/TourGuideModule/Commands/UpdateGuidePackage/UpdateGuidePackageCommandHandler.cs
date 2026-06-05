using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Errors;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateGuidePackage;

public class UpdateGuidePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageImageRepository packageImageRepository,
    IBookingRepository bookingRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
) : ICommandHandler<UpdateGuidePackageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        // 1. Load the package (tracking enabled for update)
        var package = await packageRepository.GetPackageByIdAsync(
            request.PackageId,
            new PackageQueryOptions(ReadOnly: false),
            cancellationToken);

        if (package is null)
            return GuideErrors.PackageNotFound;

        // 2. Ownership check — only the owner can update
        if (package.UserId != userId)
            return GuideErrors.UnauthorizedPackageAccess;

        // 3. Block update if there are any active (non-cancelled) bookings
        var hasActiveBookings = await bookingRepository.HasBookingsForPackageAsync(request.PackageId, cancellationToken);
        if (hasActiveBookings)
            return GuideErrors.PackageHasActiveBookings;

        // 4. Validate and parse meeting point
        var meetingPointResult = GeoPoint.Create(request.Latitude, request.Longitude);
        if (meetingPointResult.IsError) return meetingPointResult.Errors;

        // 5. Validate and parse main image URL
        var mainImageUrlResult = FileUrl.Create(request.MainImageUrl);
        if (mainImageUrlResult.IsError) return mainImageUrlResult.Errors;

        // 6. Validate and parse optional video URL
        FileUrl? mainVideoUrl = null;
        if (!string.IsNullOrWhiteSpace(request.VideoURL))
        {
            var videoUrlResult = FileUrl.Create(request.VideoURL);
            if (videoUrlResult.IsError) return videoUrlResult.Errors;
            mainVideoUrl = videoUrlResult.Value;
        }

        // 7. Validate and parse additional image URLs
        var imageUrlResults = new List<FileUrl>();
        if (request.ImageURLs?.Any() == true)
        {
            foreach (var url in request.ImageURLs)
            {
                var imageResult = FileUrl.Create(url);
                if (imageResult.IsError) return imageResult.Errors;
                imageUrlResults.Add(imageResult.Value);
            }
        }

        // 8. Validate locations exist
        foreach (var locationId in request.LocationIds)
        {
            var exists = await locationRepository.LocationExistsAsync(locationId, cancellationToken);
            if (!exists)
                return Error.Validation("Location.NotFound", $"Location {locationId} not found.");
        }

        // 9. Update core details on the domain entity
        var updateResult = package.UpdateDetails(
            request.Title,
            request.Description,
            request.DurationHours,
            request.AdultPrice,
            request.ChildPrice,
            request.TourType,
            request.MaxCapacity,
            meetingPointResult.Value,
            request.ArrivalNote,
            request.TransportType,
            request.GuestRequirements,
            request.CancellationPolicy,
            mainImageUrlResult.Value,
            mainVideoUrl);

        if (updateResult.IsError) return updateResult.Errors;

        // 10. Sync included / excluded items
        package.UpdateIncludedItems(request.IncludedIds ?? []);
        package.UpdateExcludedItems(request.ExcludedIds ?? []);

        // 11. Sync locations
        package.UpdateLocations(request.LocationIds);

        // 12. Replace activities — delete old, create new
        var existingActivities = await packageRepository.GetActivitiesByPackageIdAsync(request.PackageId, cancellationToken);
        if (existingActivities.Any())
            packageRepository.RemovePackageActivities(existingActivities);

        var newActivityIds = new List<Guid>();
        foreach (var actReq in request.Activities)
        {
            var activityResult = PackageActivity.Create(
                package.Id,
                actReq.Latitude,
                actReq.Longitude,
                actReq.Description,
                actReq.ActivityTime,
                actReq.IsOptional,
                actReq.LocationId);

            if (activityResult.IsError) return activityResult.Errors;
            newActivityIds.Add(activityResult.Value.Id);
            packageRepository.AddPackageActivities([activityResult.Value]);
        }

        package.UpdateActivities(newActivityIds);

        // 13. Replace images — delete old, create new
        var existingImages = await packageImageRepository.GetPackageImages(request.PackageId, cancellationToken);
        if (existingImages.Any())
            packageImageRepository.RemovePackageImages(existingImages);

        var newImageIds = new List<Guid>();
        if (imageUrlResults.Any())
        {
            var packageImages = imageUrlResults.Select(imgUrl => new PackageImage(package.Id, imgUrl)).ToList();
            packageImageRepository.AddPackageImages(packageImages);
            newImageIds.AddRange(packageImages.Select(img => img.Id));
        }

        package.UpdateImages(newImageIds);

        // 14. Commit all changes
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return package.Id;
    }
}
