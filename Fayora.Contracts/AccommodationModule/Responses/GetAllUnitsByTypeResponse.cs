using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.AccommodationModule.Responses
{
    public record GetAllUnitsByTypeResponse(
    string Title,
    decimal PricePerNight,
    decimal Rating,
    string MainImageUrl,
    string AddressDetails
    );
}
