using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.AccommodationModule;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;

public class CreateUnitCommandHandler(
    IClientContextProvider clientContextProvider,
    IHousingUnitRepository housingUnitRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUnitCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var ownerId = clientContextProvider.GetContext().OwnerId;

        if (ownerId is null)
        {
            return AccommodationErrors.OwnerProfileNotFound;
        }

        var housingUnit = new HousingUnit(
            ownerId.Value,
            request.Title,
            request.Description,
            request.LocationId,
            request.AddressDetails,
            request.Coordinates,
            request.Type,
            request.PricePerNight,
            request.NumberOfRooms,
            request.BedRooms,
            request.BathRooms,
            request.NumberOfBeds,
            request.MaxGuests,
            request.CheckInTime,
            request.CheckOutTime,
            request.MainImageUrl);

        if (request.AmenityIds.Count != 0)
        {
            housingUnit.AddAmenities(request.AmenityIds);
        }


        if (request.ImageUrls.Count != 0)
        {
            var images = request.ImageUrls.Select(imageUrl => new HousingUnitImage(housingUnit.Id, imageUrl));
            housingUnit.AddImages(images);
        }

        housingUnitRepository.AddUnit(housingUnit);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new Success();
    }
}