using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.ReviewModule;

namespace Fayora.Domain.Entities.ReviewModule;

public class ReviewReport : AuditableEntity<Guid>
{
    public Guid ReviewId { get; private set; }
    public Guid ReporterId { get; private set; }
    public ReportReason Reason { get; private set; }
    public string? AdditionalNotes { get; private set; }
    public bool IsResolved { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }

    private ReviewReport(
        Guid id,
        Guid reviewId,
        Guid reporterId,
        ReportReason reason,
        string? additionalNotes)
    {
        Id = id;
        ReviewId = reviewId;
        ReporterId = reporterId;
        Reason = reason;
        AdditionalNotes = additionalNotes;
        IsResolved = false;
        ResolvedAt = null;
    }

    private ReviewReport() { }

    public static Result<ReviewReport> Create(
        Guid reviewId,
        Guid reporterId,
        ReportReason reason,
        string? additionalNotes)
    {
        if (additionalNotes != null && additionalNotes.Length > 500)
        {
            return Error.Validation("ReviewReport.NotesTooLong", "Additional notes cannot exceed 500 characters.");
        }

        return new ReviewReport(
            Guid.CreateVersion7(),
            reviewId,
            reporterId,
            reason,
            additionalNotes);
    }

    public Result<Success> Resolve()
    {
        if (IsResolved)
        {
            return Error.Conflict("ReviewReport.AlreadyResolved", "This report is already resolved.");
        }

        IsResolved = true;
        ResolvedAt = DateTimeOffset.UtcNow;
        Updated();

        return Result.Success;
    }
}
