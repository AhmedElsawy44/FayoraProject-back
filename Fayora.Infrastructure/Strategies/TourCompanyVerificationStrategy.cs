using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Infrastructure.Strategies;

public class TourCompanyVerificationStrategy(ITourCompanyRepository tourCompanyRepository) : IVerificationStrategy
{
    private static readonly string EntityType = nameof(Domain.Entities.GuideModule.TourCompany);

    public bool CanHandle(string entityType) => entityType == EntityType;

    public async Task<Result<Success>> ProcessVerificationAsync(Guid entityId, bool isApproved, string adminNotes, CancellationToken ct)
    {
        var tourCompany =
            await tourCompanyRepository.GetTourCompanyByIdAsync(entityId, new GuideQueryOptions { ReadOnly = false }, ct);

        if (tourCompany is null)
            return Error.NotFound($"Tour company with ID {entityId} not found.");

        if (isApproved)
        {
            var approvalResult = tourCompany.Approve();
            if (approvalResult.IsError)
                return approvalResult;
        }
        else
        {
            var rejectionResult = tourCompany.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }

        return Result.Success;
    }
}