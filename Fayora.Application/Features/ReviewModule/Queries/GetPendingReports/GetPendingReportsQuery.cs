using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.ReviewModule;

namespace Fayora.Application.Features.ReviewModule.Queries.GetPendingReports;

public record GetPendingReportsQuery() : IQuery<Result<List<ReviewReport>>>;
