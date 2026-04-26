using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public class CreateGuidePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageImageRepository packageImageRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
    ) : ICommandHandler<CreateGuidePackageCommand, Result<CreateGuidePackageResult>>
{
    public async Task<Result<CreateGuidePackageResult>> Handle(CreateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        var tourGuideId = clientContextProvider.GetContext().UserId;
        if (tourGuideId == Guid.Empty) return TourGuideErrors.Unauthorized;

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
            request.DurationHours,
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
            var activityResult = PackageActivity.Create(package.Id, actReq.Latitude, actReq.Longitude, actReq.Description, actReq.ActivityTime, actReq.IsOptional);
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

        packageRepository.AddPackage(package);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateGuidePackageResult(package.Id);
    }
}