using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TourGuide;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;
using MediatR;

public class CreateGuidePackageCommandHandler(
    ITourGuideRepository tourGuideRepository,
    ITourGuidePackageRepository tourGuidePackageRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
    ) : IRequestHandler<CreateGuidePackageCommand, Result<CreateGuidePackageResult>>
{
    public async Task<Result<CreateGuidePackageResult>> Handle(CreateGuidePackageCommand request, CancellationToken cancellationToken)
    {
        if (Enum.TryParse<TourType>(request.TourType, true, out var tourType) == false) return TourGuideErrors.InvalidTourType;
        if (Enum.TryParse<TransportType>(request.TransportType, true, out var transportType) == false) return TourGuideErrors.InvalidTransportType;

        var tourGuideId = clientContextProvider.GetContext().TourGuideId;
        if (tourGuideId is null) return TourGuideErrors.GuidIdNotExist;

        var tourGuideExists = await tourGuideRepository.ExistsAsync(tourGuideId.Value, cancellationToken);
        if (!tourGuideExists) return TourGuideErrors.GuideNotFound;

        var meetingPoint = new GeoPoint(request.Latitude, request.Longitude);

        var packageResult = GuideTourPackage.Create(
            tourGuideId.Value,
            request.Title,
            request.Description,
            tourType,
            request.DurationHours,
            meetingPoint,
            transportType,
            request.MaxCapacity,
            request.AdultPrice,
            request.ChildPrice,
            request.ArrivalNote,
            request.MainImageUrl,
            request.VideoURL,
            request.GuestRequirements
        );

        if (packageResult.IsError) return packageResult.Errors;

        var package = packageResult.Value;

        if (request.IncludedItems?.Any() == true) package.AddIncludedItems(request.IncludedItems);
        if (request.ExcludedItems?.Any() == true) package.AddExcludedItems(request.ExcludedItems);
        if (request.ImageURLs?.Any() == true)
        {
            foreach (var url in request.ImageURLs) package.AddImage(url);
        }

        await tourGuidePackageRepository.AddPackageAsync(package, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateGuidePackageResult(package.Id);
    }
}