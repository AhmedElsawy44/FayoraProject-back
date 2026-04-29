using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Infrastructure.Strategies;

public class TourGuideVerificationStrategy(ITourGuideRepository tourGuideRepository) : IVerificationStrategy
{
    private static readonly string EntityType = nameof(Domain.Entities.GuideModule.TourGuide);
    public bool CanHandle(string entityType) => entityType == EntityType;

    public async Task<Result<Success>> ProcessVerificationAsync(Guid entityId, bool isApproved, string adminNotes, CancellationToken ct)
    {
        var tourGuide =
            await tourGuideRepository.GetGuideByIdAsync(entityId, new GuideQueryOptions { ReadOnly = false }, ct);

        if (tourGuide is null)
            return Error.NotFound($"Tour guide with ID {entityId} not found.");


        if (isApproved)
        {
            var approvalResult = tourGuide.Approve();
            if (approvalResult.IsError)
                return approvalResult;
        }
        else
        {
            var rejectionResult = tourGuide.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }
        return Result.Success;
    }
}
