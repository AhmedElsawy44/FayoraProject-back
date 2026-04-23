using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;

public class CreateUnitCommandHandler(
    IClientContextProvider clientContextProvider,
    IHousingUnitRepository housingUnitRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUnitCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(CreateUnitCommand  request, CancellationToken cancellationToken)
    {
        var ownerId = clientContextProvider.GetContext().UserId;

        var coordinates = GeoPoint.Create(request.Latitude, request.Longitude);
        if (coordinates.IsError) return coordinates.Errors;

        var mainImageResult = FileUrl.Create(request.MainImageUrl);
        if (mainImageResult.IsError) return mainImageResult.Errors;

        var imageResults = request.ImageUrls.Select(FileUrl.Create).ToList();
        var failedImage = imageResults.FirstOrDefault(r => r.IsError);
        if (failedImage is not null) return failedImage.Errors;

        var images = imageResults.Select(r => r.Value).ToList();

        var housingUnitResult = HousingUnit.Create(
            ownerId,
            request.Title,
            request.Description,
            request.LocationId,
            request.AddressDetails,
            coordinates.Value,
            request.Type,
            request.PricePerNight,
            request.NumberOfRooms,
            request.BedRooms,
            request.BathRooms,
            request.NumberOfBeds,
            request.MaxGuests,
            request.CheckInTime,
            request.CheckOutTime,
            mainImageResult.Value);

        if (housingUnitResult.IsError) return housingUnitResult.Errors;
        var housingUnit = housingUnitResult.Value;

        housingUnit.AddAmenities(request.AmenityIds);

        housingUnit.AddImages(images.Select(image => new HousingUnitImage(housingUnit.Id, image).Id));

        housingUnitRepository.AddUnit(housingUnit);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new Success();
    }
}