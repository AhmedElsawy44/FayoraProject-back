using Fayora.Domain.Common.Entity;
using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Domain.Entities.AccommodationModule;

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
    public string NationalIdUrl { get; private set; } = default!;

    private UnitOwner(Guid userId, UnitOwnerType ownerType, string nationalIdUrl, string? taxRegistrationNumber)
    {
        UserId = userId;
        OwnerType = ownerType;
        NationalIdUrl = nationalIdUrl;
        ResponseRate = 1.0f;
        AvgResponseTimeMinutes = 0;
        OwnerRating = 0f;
        IsSuperHost = false;
        CommercialName = null;
        TaxRegistrationNumber = taxRegistrationNumber;
        PreferredPayoutMethodId = null;
        VerificationStatus = VerificationStatus.Unverified;
        VerifiedAt = null;
        CancellationRate = 0f;
    }

    public static UnitOwner CreateCommercialOwner(Guid userId, string nationalIdUrl, string commercialName, string? taxRegistrationNumber)
    {
        var owner = new UnitOwner(userId, UnitOwnerType.Commercial, nationalIdUrl, taxRegistrationNumber);
        owner.CommercialName = commercialName;
        return owner;
    }

    public static UnitOwner CreateIndividualOwner(Guid userId, string nationalIdUrl)
    {
        return new UnitOwner(userId, UnitOwnerType.Individual, nationalIdUrl, null);
    }

    private UnitOwner() { }
}