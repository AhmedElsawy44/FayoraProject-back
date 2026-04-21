using Fayora.Domain.Common.Results;


namespace Fayora.Application.Features.VerificationModule.Common;

public static class VerificationErrors
{
    public static Error PendingRequestAlreadyExists = Error.Validation(
        "PendingRequestAlreadyExists",
        "A pending verification request of the same type already exists for this user."
    );

    public static Error VerificationRequestNotFound(int requestId) => Error.NotFound(
        code: "Verification.NotFound",
        description: $"Verification request {requestId} not found.");

    public static readonly Error InvalidReviewStatus = Error.Validation(
        code: "Verification.InvalidStatus",
        description: "Status must be either Approved or Rejected.");
}
