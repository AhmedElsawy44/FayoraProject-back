using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.SharedModule
{
    public class DiscountOffer : AuditableEntity<Guid>
    {
        public Guid OwnerId { get; private set; } // unitOwner or TourGuide 
        public Guid TargetId { get; private set; } // HousingUnit.Id, GuidePackage.Id or TourGuide.Id
        public OfferTargetType TargetType { get; private set; }

        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }

        public DiscountType DiscountType { get; private set; }
        public decimal DiscountValue { get; private set; }

        public DateTimeOffset StartDate { get; private set; }
        public DateTimeOffset EndDate { get; private set; }

        public DiscountOfferStatus Status { get; private set; }

        private DiscountOffer() { }

        private DiscountOffer(
            Guid ownerId,
            Guid targetId,
            OfferTargetType targetType,
            string title,
            string? description,
            DiscountType discountType,
            decimal discountValue,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            Id = Guid.CreateVersion7();
            OwnerId = ownerId;
            TargetId = targetId;
            TargetType = targetType;
            Title = title;
            Description = description;
            DiscountType = discountType;
            DiscountValue = discountValue;
            StartDate = startDate;
            EndDate = endDate;
            Status = DiscountOfferStatus.Active;
        }

        public static Result<DiscountOffer> Create(
            Guid ownerId,
            Guid targetId,
            OfferTargetType targetType,
            string title,
            string? description,
            DiscountType discountType,
            decimal discountValue,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            if (ownerId == Guid.Empty)
                return Error.Validation("DiscountOffer.OwnerId", "Owner ID is required.");

            if (targetId == Guid.Empty)
                return Error.Validation("DiscountOffer.TargetId", "Target ID is required.");

            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("DiscountOffer.Title", "Title is required.");

            if (title.Length > 100)
                return Error.Validation("DiscountOffer.Title", "Title must not exceed 100 characters.");

            if (discountValue <= 0)
                return Error.Validation("DiscountOffer.DiscountValue", "Discount value must be greater than zero.");

            if (discountType == DiscountType.Percentage && discountValue > 100)
                return Error.Validation("DiscountOffer.DiscountValue", "Percentage discount cannot exceed 100%.");

            if (startDate >= endDate)
                return Error.Validation("DiscountOffer.Dates", "Start date must be before end date.");

            if (endDate <= DateTimeOffset.UtcNow)
                return Error.Validation("DiscountOffer.EndDate", "End date must be in the future.");

            return new DiscountOffer(ownerId, targetId, targetType, title, description,
                discountType, discountValue, startDate, endDate);
        }

        public Result<decimal> ApplyTo(decimal originalPrice)
        {
            if (!IsCurrentlyValid())
                return Error.Validation("DiscountOffer.NotValid", "This offer is not currently active.");

            var discounted = DiscountType == DiscountType.Percentage
                ? originalPrice - (originalPrice * DiscountValue / 100)
                : originalPrice - DiscountValue;

            if (discounted < 0)
                return Error.Validation("DiscountOffer.ExceedsPrice", "Discount exceeds the original price.");

            return discounted;
        }

        public Result<Success> Cancel()
        {
            if (Status != DiscountOfferStatus.Active)
                return Error.Conflict("DiscountOffer.NotActive", "Only active offers can be cancelled.");

            Status = DiscountOfferStatus.Cancelled;
            Updated();
            return Result.Success;
        }

        public void ExpireIfNeeded()
        {
            if (Status == DiscountOfferStatus.Active && DateTimeOffset.UtcNow > EndDate)
            {
                Status = DiscountOfferStatus.Expired;
                Updated();
            }
        }

        public bool IsCurrentlyValid() =>
            Status == DiscountOfferStatus.Active &&
            DateTimeOffset.UtcNow >= StartDate &&
            DateTimeOffset.UtcNow <= EndDate;
    }
}
