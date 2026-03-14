using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.ValueObjects;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;

public record CreateUnitCommand(
    string Title,
    string? Description,
    int LocationId,
    string AddressDetails,
    GeoPoint Coordinates,
    HousingType Type,
    decimal PricePerNight,
    int NumberOfRooms,
    int BedRooms,
    int BathRooms,
    int NumberOfBeds,
    int MaxGuests,
    TimeSpan CheckInTime,
    TimeSpan CheckOutTime,
    string MainImageUrl,
    Guid VerificationRequestId,
    HashSet<string> ImageUrls,
    HashSet<int> AmenityIds) : IRequest<Result<Success>>;
