using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;

namespace Fayora.Infrastructure.Strategies;

public class GuidePackageVerificationStrategy(IPackageRepository packageRepository) : IVerificationStrategy
{
    private static readonly string EntityType = nameof(Domain.Entities.GuideModule.GuidePackage);

    public bool CanHandle(string entityType) => entityType == EntityType;

    public async Task<Result<Success>> ProcessVerificationAsync(Guid entityId, bool isApproved, string adminNotes, CancellationToken ct)
    {
        var guidePackage =
            await packageRepository.GetPackageByIdAsync(entityId, new PackageQueryOptions { ReadOnly = false }, ct);

        if (guidePackage is null)
            return Error.NotFound($"Guide package with ID {entityId} not found.");

        if (isApproved)
        {
            var approvalResult = guidePackage.Approve();
            if (approvalResult.IsError)
                return approvalResult;
        }
        else
        {
            var rejectionResult = guidePackage.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }

        return Result.Success;
    }
}