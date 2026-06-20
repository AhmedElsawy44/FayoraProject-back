using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.NotificationModule;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Strategies;

public class TourCompanyVerificationStrategy(
    ITourCompanyRepository tourCompanyRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFirebaseNotificationService firebaseNotificationService) : IVerificationStrategy
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

            var user = await userRepository.GetUserByIdAsync(tourCompany.UserId, new UserQueryOptions { IsReadOnly = false }, ct);
            user?.AddRole(Role.TourCompany);
        }
        else
        {
            var rejectionResult = tourCompany.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }

        var userForNotify = await userRepository.GetUserByIdAsync(tourCompany.UserId, new UserQueryOptions { IsReadOnly = true }, ct);
        bool isArabic = userForNotify?.PreferredLanguage == Language.Arabic;

        string title = isApproved 
            ? (isArabic ? "تم قبول طلب التحقق!" : "Verification Approved!") 
            : (isArabic ? "تم رفض طلب التحقق" : "Verification Rejected");

        string body = isApproved 
            ? (isArabic ? "تهانينا! تم قبول طلب تسجيل شركتك السياحية." : "Congratulations! Your request to register your Tour Company has been approved.") 
            : (isArabic ? $"تم رفض طلب التحقق لشركتك. السبب: {adminNotes}" : $"Your company verification request was rejected. Reason: {adminNotes}");

        // Save In-App Notification in DB
        var inAppNotification = InAppNotification.Create(
            tourCompany.UserId,
            title,
            body,
            "verification_update",
            entityId.ToString()
        );
        await notificationRepository.AddInAppNotificationAsync(inAppNotification, ct);

        var tokens = await notificationRepository.GetTokensByUserIdAsync(tourCompany.UserId, ct);
        if (tokens.Any())
        {
            var dataPayload = new Dictionary<string, string>
            {
                { "type", "verification_update" },
                { "entityType", "TourCompany" },
                { "isApproved", isApproved.ToString().ToLower() }
            };

            await firebaseNotificationService.SendBroadcastAsync(title, body, null, tokens, ct, dataPayload);
        }

        return Result.Success;
    }
}