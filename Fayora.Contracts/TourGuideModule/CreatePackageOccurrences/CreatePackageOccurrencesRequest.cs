using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.TourGuideModule.CreatePackageOccurrences
{
    public record CreatePackageOccurrencesRequest(
        List<OccurrenceItemRequest> Occurrences
    );

    public record OccurrenceItemRequest(
        DateTime Date,
        int AvailableSeats
    );

}
