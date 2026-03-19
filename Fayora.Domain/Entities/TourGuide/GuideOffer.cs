using Fayora.Domain.Common.Entity;

namespace Fayora.Domain.Entities.TourGuide
{


    public class GuideOffer : AuditableEntity<Guid>
    {
        public Guid RequestId { get; private set; }
        public Guid GuideId { get; private set; }
        public decimal ProposedPrice { get; private set; }
        public string CurrencyCode { get; private set; } = null!;
        public string? Message { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }
        public GuideOfferStatus Status { get; private set; }

        private GuideOffer(
            Guid requestId,
            Guid guideId,
            decimal proposedPrice,
            string currencyCode,
            string? message,
            DateTimeOffset? expiresAt)
        {
            RequestId = requestId;
            GuideId = guideId;
            ProposedPrice = proposedPrice;
            CurrencyCode = currencyCode;
            Message = message;
            ExpiresAt = expiresAt;
            Status = GuideOfferStatus.Pending;
        }

        private GuideOffer() { }

        public static GuideOffer CreateGuideOffer(
            Guid requestId,
            Guid guideId,
            decimal proposedPrice,
            string currencyCode = "EGP",
            string? message = null,
            DateTimeOffset? expiresAt = null)
        {
            if (proposedPrice <= 0)
                throw new InvalidOperationException("Proposed price must be greater than zero.");

            return new GuideOffer(requestId, guideId, proposedPrice, currencyCode, message, expiresAt);
        }

        public void Accept()
        {
            if (Status != GuideOfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can be accepted.");
            if (IsExpired())
                throw new InvalidOperationException("Cannot accept an expired offer.");
            Status = GuideOfferStatus.Accepted;
            Updated();
        }

        public void Reject()
        {
            if (Status != GuideOfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can be rejected.");
            if (IsExpired())
                throw new InvalidOperationException("Cannot reject an expired offer.");
            Status = GuideOfferStatus.Rejected;
            Updated();
        }

        public void Withdraw()
        {
            if (Status != GuideOfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can be withdrawn.");
            Status = GuideOfferStatus.Withdrawn;
            Updated();
        }

        public void Expire()
        {
            if (Status != GuideOfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can expire.");
            Status = GuideOfferStatus.Expired;
            Updated();
        }


        public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow;

        public void UpdatePrice(decimal newPrice)
        {
            if (Status != GuideOfferStatus.Pending)
                throw new InvalidOperationException("Cannot update price for a closed offer.");
            if (newPrice <= 0)
                throw new InvalidOperationException("Price must be greater than zero.");
            ProposedPrice = newPrice;
            Updated();
        }






    }

}
