using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public class CreateGuidePackageCommandHandler(
    IPackageRepository packageRepository,
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
                var image = FileUrl.Create(url);
                if (image.IsError) return image.Errors;
                imageUrlResults.Add(image.Value);
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
            request.GuestRequirements
        );

        if (packageResult.IsError) return packageResult.Errors;
        var package = packageResult.Value;

        if (request.IncludedIds?.Any() == true) package.AddIncludedItems(request.IncludedIds);
        if (request.ExcludedIds?.Any() == true) package.AddExcludedItems(request.ExcludedIds);

        foreach (var imageUrlResult in imageUrlResults)
        {
            package.AddImage(imageUrlResult);
        }

        packageRepository.AddPackage(package, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateGuidePackageResult(package.Id);
    }
}