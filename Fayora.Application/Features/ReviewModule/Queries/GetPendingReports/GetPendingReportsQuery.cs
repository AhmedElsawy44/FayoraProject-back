using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Queries.GetPendingReports;

public record ReviewReportDto(
    Guid Id,
    Guid ReviewId,
    Guid ReporterId,
    string Reason,
    string? AdditionalNotes,
    bool IsResolved,
    DateTimeOffset CreatedAt,
    string AuthorName,
    Guid TargetId,
    string TargetType,
    string TargetName,
    decimal Rating,
    string? Comment,
    DateTimeOffset ReviewCreatedAt
);

public record GetPendingReportsQuery() : IQuery<Result<List<ReviewReportDto>>>;
