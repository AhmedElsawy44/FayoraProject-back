using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public class CreateGuidePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageImageRepository packageImageRepository,
    IPackageAccommodationRepository packageAccommodationRepository,
    IPackageNightRepository packageNightRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
    ) : ICommandHandler<CreateGuidePackageCommand, Result<CreateGuidePackageResult>>
{
    public async Task<Result<CreateGuidePackageResult>> Handle(CreateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var tourGuideId = clientContextProvider.GetContext().UserId;

        var providerType = clientContextProvider.GetContext().Roles.Contains("TourGuide") ? ProviderType.TourGuide : ProviderType.TourCompany;

        var meetingPointResult = GeoPoint.Create(request.Latitude, request.Longitude);
        if (meetingPointResult.IsError) return meetingPointResult.Errors;
        var meetingPoint = meetingPointResult.Value;

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


        var packageResult = GuidePackage.Create(
            tourGuideId,
            request.Title,
            request.Description,
            request.TourType,
            providerType,
            request.DurationHours,
            request.NumOfDays,
            meetingPoint,
            request.TransportType,
            request.MaxCapacity,
            request.AdultPrice,
            request.ChildPrice,
            request.ArrivalNote,
            mainImageUrlResult.Value,
            mainVideoUrl,
            request.GuestRequirements,
            request.CancellationPolicy
        );


        if (packageResult.IsError) return packageResult.Errors;
        var package = packageResult.Value;

        var actualActivities = new List<Guid>();
        foreach (var actReq in request.Activities)
        {
            var activityResult = PackageActivity.Create(package.Id, actReq.Latitude, actReq.Longitude, actReq.Description, actReq.ActivityTime, actReq.IsOptional, actReq.LocationId);
            if (activityResult.IsError) return activityResult.Errors;
            actualActivities.Add(activityResult.Value.Id);
        }

        package.AddActivities(actualActivities);

        if (request.IncludedIds?.Any() == true) package.AddIncludedItems(request.IncludedIds);
        if (request.ExcludedIds?.Any() == true) package.AddExcludedItems(request.ExcludedIds);

        if (imageUrlResults.Any())
        {
            var packageImages = imageUrlResults.Select(imgUrl => new PackageImage(package.Id, imgUrl)).ToList();
            package.AddImages(packageImages.Select(img => img.Id));
            packageImageRepository.AddPackageImages(packageImages);
        }


        // Nights
        if (request.NumOfDays > 1 && (request.Nights == null || !request.Nights.Any()))
            return Error.Validation("Package.NightsRequired",
                "Multi-day packages must include accommodation for each night.");

        if (request.NumOfDays == 1 && request.Nights?.Any() == true)
            return Error.Validation("Package.NoNightsAllowed",
                "Single-day packages cannot have accommodation nights.");

        if (request.NumOfDays > 1 && request.Nights?.Count != package.NumOfNights)
            return Error.Validation("Package.InvalidNightsCount",
                $"Number of nights must be exactly {package.NumOfNights}.");

        if (request.Nights?.Any() == true)
        {
            if (request.Nights.Count > package.NumOfNights)
                return Error.Validation("Package.TooManyNights",
                    "Number of nights cannot exceed NumOfDays - 1.");

            var nightIds = new List<Guid>();
            var createdAccommodations = new Dictionary<string, Guid>(); // Key: "Name|Type", Value: AccommodationId , to avoid creating duplicate accommodations with the same name and type in more than one night

            foreach (var nightReq in request.Nights)
            {
                Guid? accommodationId = null;

                if (nightReq.NewAccommodation is not null)
                {
                    var key = $"{nightReq.NewAccommodation.Name.Trim().ToLower()}_{nightReq.NewAccommodation.Type}";

                    if (createdAccommodations.TryGetValue(key, out var existingAccommodationId))
                    {
                        //the same accommodation was created before in the same request
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

            package.AddNights(nightIds);
        }



        foreach (var locationId in request.LocationIds)
        {
            var exists = await locationRepository.LocationExistsAsync(locationId, cancellationToken);
            if (!exists)
                return Error.Validation("Location.NotFound", $"Location {locationId} not found.");
        }


        package.AddLocations(request.LocationIds);

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

        packageRepository.AddPackage(package);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateGuidePackageResult(package.Id);
    }
}