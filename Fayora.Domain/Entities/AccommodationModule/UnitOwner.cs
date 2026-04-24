using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Domain.Entities.AccommodationModule;

public class UnitOwner : AggregateRoot
{
    public Guid UserId { get; init; }
    public UnitOwnerType OwnerType { get; private set; }
    public float ResponseRate { get; private set; }
    public int AvgResponseTimeMinutes { get; private set; }
    public float OwnerRating { get; private set; }
    public bool IsSuperHost { get; private set; }
    public string? CommercialName { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public float CancellationRate { get; private set; }
    public Guid? PreferredPayoutMethodId { get; private set; }

    public static UnitOwner CreateCommercialOwner(Guid userId, string commercialName)
    {
        return new UnitOwner
        {
            UserId = userId,
            OwnerType = UnitOwnerType.Commercial,
            CommercialName = commercialName,
        };
    }

    public static UnitOwner CreateIndividualOwner(Guid userId, string commercialName)
    {
        return new UnitOwner
        {
            UserId = userId,
            OwnerType = UnitOwnerType.Individual,
            CommercialName = commercialName,
        };
    }

    private UnitOwner() { }
}