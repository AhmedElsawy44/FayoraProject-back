using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.ReportReview;

public record ReportReviewCommand(Guid ReviewId, string Reason, string? AdditionalNotes) : ICommand<Result<Success>>;
