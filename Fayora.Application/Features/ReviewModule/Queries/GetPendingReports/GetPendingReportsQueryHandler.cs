using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.ReviewModule;

namespace Fayora.Application.Features.ReviewModule.Queries.GetPendingReports;

public class GetPendingReportsQueryHandler(IReviewRepository reviewRepository)
    : IQueryHandler<GetPendingReportsQuery, Result<List<ReviewReport>>>
{
    public async Task<Result<List<ReviewReport>>> Handle(
        GetPendingReportsQuery request,
        CancellationToken cancellationToken)
    {
        var reports = await reviewRepository.GetPendingReportsAsync(cancellationToken);
        return reports;
    }
}
