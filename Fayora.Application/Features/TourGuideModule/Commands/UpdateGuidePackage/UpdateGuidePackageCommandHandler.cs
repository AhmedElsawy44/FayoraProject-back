using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.Errors;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateGuidePackage;

public class UpdateGuidePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageImageRepository packageImageRepository,
    IBookingRepository bookingRepository,
    IPackageAccommodationRepository packageAccommodationRepository,
    IPackageNightRepository packageNightRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
) : ICommandHandler<UpdateGuidePackageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;


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


        var meetingPointResult = GeoPoint.Create(request.Latitude, request.Longitude);
        if (meetingPointResult.IsError) return meetingPointResult.Errors;


        var mainImageUrlResult = FileUrl.Create(request.MainImageUrl);
        if (mainImageUrlResult.IsError) return mainImageUrlResult.Errors;


        FileUrl? mainVideoUrl = null;
        if (!string.IsNullOrWhiteSpace(request.VideoURL))
        {
            var videoUrlResult = FileUrl.Create(request.VideoURL);
            if (videoUrlResult.IsError) return videoUrlResult.Errors;
            mainVideoUrl = videoUrlResult.Value;
        }


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


        foreach (var locationId in request.LocationIds)
        {
            var exists = await locationRepository.LocationExistsAsync(locationId, cancellationToken);
            if (!exists)
                return Error.Validation("Location.NotFound", $"Location {locationId} not found.");
        }


        var updateResult = package.UpdateDetails(
            request.Title,
            request.Description,
            request.DurationHours,
            request.NumOfDays,
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


        package.UpdateIncludedItems(request.IncludedIds ?? []);
        package.UpdateExcludedItems(request.ExcludedIds ?? []);

 
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


        // Nights validations
        if (request.NumOfDays > 1 && (request.Nights == null || !request.Nights.Any()))
            return Error.Validation("Package.NightsRequired",
                "Multi-day packages must include accommodation for each night.");

        if (request.NumOfDays == 1 && request.Nights?.Any() == true)
            return Error.Validation("Package.NoNightsAllowed",
                "Single-day packages cannot have accommodation nights.");

        var expectedNightsCount = request.NumOfDays > 1 ? request.NumOfDays - 1 : 0;
        if (request.NumOfDays > 1 && request.Nights?.Count != expectedNightsCount)
            return Error.Validation("Package.InvalidNightsCount",
                $"Number of nights must be exactly {expectedNightsCount}.");

        if (request.Nights?.Any() == true && request.Nights.Count > expectedNightsCount)
            return Error.Validation("Package.TooManyNights",
                "Number of nights cannot exceed NumOfDays - 1.");

        // remove existing nights and accommodations first
        var existingNights = await packageNightRepository.GetByPackageIdAsync(
            request.PackageId, cancellationToken);
        if (existingNights.Any())
            packageNightRepository.RemoveNights(existingNights);

        var existingAccommodations = await packageAccommodationRepository.GetByPackageIdAsync(
            request.PackageId, cancellationToken);
        if (existingAccommodations.Any())
            packageAccommodationRepository.RemoveAccommodations(existingAccommodations);

        // add new nights and accommodations
        if (request.Nights?.Any() == true)
        {
            var nightIds = new List<Guid>();
            var createdAccommodations = new Dictionary<string, Guid>();

            foreach (var nightReq in request.Nights)
            {
                Guid? accommodationId = null;

                if (nightReq.NewAccommodation is not null)
                {
                    var key = $"{nightReq.NewAccommodation.Name.Trim().ToLower()}_{nightReq.NewAccommodation.Type}";

                    if (createdAccommodations.TryGetValue(key, out var existingAccommodationId))
                    {
                        accommodationId = existingAccommodationId;
                    }
                    else
                    {
                        if (!Enum.TryParse<PackageAccommodationType>(nightReq.NewAccommodation.Type, out var accommodationType))
                            return Error.Validation("PackageAccommodation.InvalidType", "Invalid accommodation type.");

                        PackageAmenities combinedAmenities = PackageAmenities.None;
                        if (nightReq.NewAccommodation.Amenities?.Any() == true)
                        {
                            foreach (var amenity in nightReq.NewAccommodation.Amenities)
                            {
                                if (Enum.TryParse<PackageAmenities>(amenity, out var amenityValue))
                                    combinedAmenities |= amenityValue;
                            }
                        }

                        PackageMeals combinedMeals = PackageMeals.None;
                        if (nightReq.NewAccommodation.Meals?.Any() == true)
                        {
                            foreach (var meal in nightReq.NewAccommodation.Meals)
                            {
                                if (Enum.TryParse<PackageMeals>(meal, out var mealValue))
                                    combinedMeals |= mealValue;
                            }
                        }

                        var accommodationResult = PackageAccommodation.Create(
                            package.Id,
                            nightReq.NewAccommodation.Name,
                            nightReq.NewAccommodation.Description,
                            accommodationType,
                            nightReq.NewAccommodation.MainImageUrl,
                            nightReq.NewAccommodation.Latitude,
                            nightReq.NewAccommodation.Longitude,
                            nightReq.NewAccommodation.CheckInTime,
                            nightReq.NewAccommodation.CheckOutTime,
                            combinedAmenities,
                            combinedMeals,
                            nightReq.NewAccommodation.GalleryImages);

                        if (accommodationResult.IsError) return accommodationResult.Errors;
                        accommodationId = accommodationResult.Value.Id;
                        createdAccommodations[key] = accommodationId.Value;
                        packageAccommodationRepository.Add(accommodationResult.Value);
                    }
                }

                var nightResult = PackageNight.Create(
                    package.Id,
                    nightReq.NightNumber,
                    nightReq.NightDate,
                    nightReq.HousingUnitId,
                    accommodationId);

                if (nightResult.IsError) return nightResult.Errors;
                nightIds.Add(nightResult.Value.Id);
                packageNightRepository.Add(nightResult.Value);
            }

            package.UpdateNights(nightIds);
        }
        else
        {
            package.UpdateNights([]);
        }


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

        var optionalActivities = new List<OptionalActivity>();
        if (request.OptionalActivities?.Any() == true)
        {
            foreach (var optAct in request.OptionalActivities)
            {
                var optActResult = OptionalActivity.Create(optAct.Description, optAct.AdditionalPrice, optAct.ImageUrl);
                if (optActResult.IsError) return optActResult.Errors;
                optionalActivities.Add(optActResult.Value);
            }
        }
        package.UpdateOptionalActivities(optionalActivities);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return package.Id;
    }
}
