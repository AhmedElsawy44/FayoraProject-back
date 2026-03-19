namespace Fayora.Domain.Enums.TourGuideModule
{
    public enum GuideRequestStatus
    {
        Open,      // Request is open and accepting offers
        Accepted,  // Tourist  accepted offer
        Cancelled, // Tourist cancelled the request
        Expired    // Request expired without any accepted offer
    }
}

