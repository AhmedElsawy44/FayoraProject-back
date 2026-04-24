using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Domain.Entities.GuideModule
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

        public static Result<GuideOffer> Create(
            Guid requestId, Guid guideId, decimal proposedPrice,
            string currencyCode = "EGP", string? message = null,
            DateTimeOffset? expiresAt = null)
        {
            if (proposedPrice <= 0)
                return Error.Validation("GuideOffer.InvalidPrice", "Proposed price must be greater than zero.");

            return new GuideOffer(requestId, guideId, proposedPrice, currencyCode, message, expiresAt);
        }

        public Result<Success> Accept()
        {
            if (Status != GuideOfferStatus.Pending)
                return Error.Conflict("GuideOffer.NotPending", "Only pending offers can be accepted.");
            if (IsExpired())
                return Error.Conflict("GuideOffer.Expired", "Cannot accept an expired offer.");
            Status = GuideOfferStatus.Accepted;
            Updated();
            return Result.Success;
        }

        public Result<Success> Reject()
        {
            if (Status != GuideOfferStatus.Pending)
                return Error.Conflict("GuideOffer.NotPending", "Only pending offers can be rejected.");
            Status = GuideOfferStatus.Rejected;
            Updated();
            return Result.Success;
        }

        public Result<Success> Withdraw()
        {
            if (Status != GuideOfferStatus.Pending)
                return Error.Conflict("GuideOffer.NotPending", "Only pending offers can be withdrawn.");
            Status = GuideOfferStatus.Withdrawn;
            Updated();
            return Result.Success;
        }

        public Result<Success> Expire()
        {
            if (Status != GuideOfferStatus.Pending)
                return Error.Conflict("GuideOffer.NotPending", "Only pending offers can expire.");
            Status = GuideOfferStatus.Expired;
            Updated();
            return Result.Success;
        }

        public Result<Success> UpdatePrice(decimal newPrice)
        {
            if (Status != GuideOfferStatus.Pending)
                return Error.Conflict("GuideOffer.NotPending", "Cannot update price for a closed offer.");
            if (newPrice <= 0)
                return Error.Validation("GuideOffer.InvalidPrice", "Price must be greater than zero.");
            ProposedPrice = newPrice;
            Updated();
            return Result.Success;
        }



        public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow;








    }

}
