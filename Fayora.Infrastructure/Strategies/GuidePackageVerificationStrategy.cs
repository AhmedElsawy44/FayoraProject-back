using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Strategies;

public class GuidePackageVerificationStrategy(
    IPackageRepository packageRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFirebaseNotificationService firebaseNotificationService) : IVerificationStrategy
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

        var tokens = await notificationRepository.GetTokensByUserIdAsync(guidePackage.UserId, ct);
        if (tokens.Any())
        {
            var user = await userRepository.GetUserByIdAsync(guidePackage.UserId, new UserQueryOptions { IsReadOnly = true }, ct);
            bool isArabic = user?.PreferredLanguage == Language.Arabic;

            string title = isApproved 
                ? (isArabic ? "تم قبول باقة الرحلة!" : "Tour Package Approved!") 
                : (isArabic ? "تم رفض باقة الرحلة" : "Tour Package Rejected");

            string body = isApproved 
                ? (isArabic ? $"تم قبول باقة الرحلة الخاصة بك '{guidePackage.Title}' بنجاح." : $"Your tour package '{guidePackage.Title}' has been approved.") 
                : (isArabic ? $"تم رفض باقة الرحلة الخاصة بك '{guidePackage.Title}'. السبب: {adminNotes}" : $"Your tour package '{guidePackage.Title}' was rejected. Reason: {adminNotes}");

            await firebaseNotificationService.SendBroadcastAsync(title, body, null, tokens, ct);
        }

        return Result.Success;
    }
}