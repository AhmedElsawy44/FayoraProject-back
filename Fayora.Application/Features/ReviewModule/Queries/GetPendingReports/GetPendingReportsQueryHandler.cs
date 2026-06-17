using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Queries.GetPendingReports;

public class GetPendingReportsQueryHandler(IReviewRepository reviewRepository)
    : IQueryHandler<GetPendingReportsQuery, Result<List<ReviewReportDto>>>
{
    public async Task<Result<List<ReviewReportDto>>> Handle(
        GetPendingReportsQuery request,
        CancellationToken cancellationToken)
    {
        var reports = await reviewRepository.GetPendingReportsWithDetailsAsync(cancellationToken);
        return reports;
    }
}
