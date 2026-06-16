using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Strategies;

public class TourGuideVerificationStrategy(
    ITourGuideRepository tourGuideRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFirebaseNotificationService firebaseNotificationService) : IVerificationStrategy
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

            var user = await userRepository.GetUserByIdAsync(tourGuide.UserId, new UserQueryOptions { IsReadOnly = false }, ct);
            user?.AddRole(Role.TourGuide);
        }
        else
        {
            var rejectionResult = tourGuide.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }

        var tokens = await notificationRepository.GetTokensByUserIdAsync(tourGuide.UserId, ct);
        if (tokens.Any())
        {
            var user = await userRepository.GetUserByIdAsync(tourGuide.UserId, new UserQueryOptions { IsReadOnly = true }, ct);
            bool isArabic = user?.PreferredLanguage == Language.Arabic;

            string title = isApproved 
                ? (isArabic ? "تم قبول طلب التحقق!" : "Verification Approved!") 
                : (isArabic ? "تم رفض طلب التحقق" : "Verification Rejected");

            string body = isApproved 
                ? (isArabic ? "تهانينا! تم قبول طلبك لتصبح مرشداً سياحياً." : "Congratulations! Your request to become a Tour Guide has been approved.") 
                : (isArabic ? $"تم رفض طلب التحقق الخاص بك. السبب: {adminNotes}" : $"Your verification request was rejected. Reason: {adminNotes}");

            await firebaseNotificationService.SendBroadcastAsync(title, body, null, tokens, ct);
        }

        return Result.Success;
    }
}
