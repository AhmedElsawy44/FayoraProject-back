using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Commands.ResolveReport;

public class ResolveReportCommandHandler(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ResolveReportCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        ResolveReportCommand request,
        CancellationToken cancellationToken)
    {
        var report = await reviewRepository.GetReportByIdAsync(request.ReportId, cancellationToken);
        if (report is null)
        {
            return ReviewErrors.ReportNotFound;
        }

        var resolveResult = report.Resolve();
        if (resolveResult.IsError)
        {
            return resolveResult.Errors;
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
