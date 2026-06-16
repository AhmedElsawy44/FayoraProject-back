using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Strategies;

public class HousingUnitVerificationStrategy(
    IHousingUnitRepository housingUnitRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFirebaseNotificationService firebaseNotificationService) : IVerificationStrategy
{
    public bool CanHandle(string entityType) =>
        entityType.Equals("Accommodation", StringComparison.OrdinalIgnoreCase) ||
        entityType.Equals("HousingUnit", StringComparison.OrdinalIgnoreCase);

    public async Task<Result<Success>> ProcessVerificationAsync(Guid entityId, bool isApproved, string adminNotes, CancellationToken ct)
    {
        var housingUnit = await housingUnitRepository.GetUnitByIdAsync(entityId, new UnitQueryOptions(IsReadOnly: false), ct);

        if (housingUnit is null)
            return Error.NotFound($"Housing unit with ID {entityId} not found.");

        if (isApproved)
        {
            var approvalResult = housingUnit.Approve();
            if (approvalResult.IsError)
                return approvalResult;

            var user = await userRepository.GetUserByIdAsync(housingUnit.OwnerId, new UserQueryOptions { IsReadOnly = false }, ct);
            user?.AddRole(Role.UnitOwner);
        }
        else
        {
            var rejectionResult = housingUnit.Reject(adminNotes);
            if (rejectionResult.IsError)
                return rejectionResult;
        }

        var tokens = await notificationRepository.GetTokensByUserIdAsync(housingUnit.OwnerId, ct);
        if (tokens.Any())
        {
            var user = await userRepository.GetUserByIdAsync(housingUnit.OwnerId, new UserQueryOptions { IsReadOnly = true }, ct);
            bool isArabic = user?.PreferredLanguage == Language.Arabic;

            string title = isApproved 
                ? (isArabic ? "تم قبول الوحدة السكنية!" : "Housing Unit Approved!") 
                : (isArabic ? "تم رفض الوحدة السكنية" : "Housing Unit Rejected");

            string body = isApproved 
                ? (isArabic ? $"تم قبول وحدتك السكنية '{housingUnit.Title}' بنجاح." : $"Your housing unit '{housingUnit.Title}' has been approved.") 
                : (isArabic ? $"تم رفض وحدتك السكنية '{housingUnit.Title}'. السبب: {adminNotes}" : $"Your housing unit '{housingUnit.Title}' was rejected. Reason: {adminNotes}");

            await firebaseNotificationService.SendBroadcastAsync(title, body, null, tokens, ct);
        }

        return Result.Success;
    }
}
