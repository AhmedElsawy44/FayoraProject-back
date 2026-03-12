using Fayora.Domain.Common.Entity;
using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Domain.Entitties.Accommodation;

public class UnitOwner : AuditableEntity<Guid>
{
    public Guid UserId { get; init; }
    public UnitOwnerType OwnerType { get; private set; }
    public float ResponseRate { get; private set; }
    public int AvgResponseTimeMinutes { get; private set; }
    public float OwnerRating { get; private set; }
    public bool IsSuperHost { get; private set; }
    public string? CommercialName { get; private set; }
    public string? TaxRegistrationNumber { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public float CancellationRate { get; private set; }
    public Guid? PreferredPayoutMethodId { get; private set; }

    public UnitOwner(Guid userId, UnitOwnerType ownerType, Guid preferredPayoutMethodId)
    {
        UserId = userId;
        OwnerType = ownerType;
        PreferredPayoutMethodId = preferredPayoutMethodId;
        ResponseRate = 1.0f; 
        AvgResponseTimeMinutes = 0;
        OwnerRating = 0f;
        IsSuperHost = false;
        CommercialName = null;
        TaxRegistrationNumber = null;
        VerificationStatus = VerificationStatus.Unverified;
        VerifiedAt = null;
        CancellationRate = 0f;
    }

    private UnitOwner() { }
}