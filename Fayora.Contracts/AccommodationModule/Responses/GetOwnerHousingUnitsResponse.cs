using System;
using System.Collections.Generic;

namespace Fayora.Contracts.AccommodationModule.Responses
{
    public record GetOwnerHousingUnitsResponse(
        Guid Id,
        string Title,
        string Description,
        int LocationId,
        string AddressDetails,
        double Latitude,
        double Longitude,
        string Type,
        decimal PricePerNight,
        int NumberOfRooms,
        int BedRooms,
        int BathRooms,
        int NumberOfBeds,
        int MaxGuests,
        string CheckInTime,
        string CheckOutTime,
        string MainImageUrl,
        string? VerificationDocumentUrl,
        List<string> ImageUrls,
        List<string> Amenities,
        string Status,
        DateTime? AvailableStartDate,
        DateTime? AvailableEndDate
    );
}
