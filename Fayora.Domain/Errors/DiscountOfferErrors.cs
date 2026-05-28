using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Errors
{
    public static class DiscountOfferErrors
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("DiscountOffer.NotFound", $"Offer with ID '{id}' was not found.");

        public static readonly Error Unauthorized =
            Error.Forbidden("DiscountOffer.Unauthorized", "You are not allowed to manage this offer.");

        public static readonly Error ActiveOfferAlreadyExists =
            Error.Conflict("DiscountOffer.AlreadyExists", "An active offer already exists for this target.");

        public static readonly Error TargetNotFound =
            Error.NotFound("DiscountOffer.TargetNotFound", "The target entity was not found.");
    }
}
