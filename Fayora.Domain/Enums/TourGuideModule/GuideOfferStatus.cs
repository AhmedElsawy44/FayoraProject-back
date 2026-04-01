namespace Fayora.Domain.Entities.TourGuide
{
    public enum GuideOfferStatus
    {
        Pending,   // Offer sent and waiting for tourist response
        Accepted,  // Tourist accepted the offer
        Rejected,  // Tourist rejected the offer
        Expired,   // Offer expired without response
        Withdrawn  // Guide withdrew the offer
    }
}