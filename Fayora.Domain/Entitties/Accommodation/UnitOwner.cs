using Fayora.Domain.Common.Entity;

namespace Fayora.Domain.Entitties.Accommodation;

public class UnitOwner : AuditableEntity<Guid>
{
    public Guid UserId { get; init; }
    public float ResponseRate { get; private set; }
    public int AvgResponseTimeMinutes { get; private set; }
    public float OwnerRating { get; private set; }
    public bool IsSuperHost { get; private set; }
    public int TotalUnitsListed { get; private set; }
    public string? CommercialName { get; private set; }
    public string? TaxRegistrationNumber { get; private set; }
    public bool IsListingEnabled { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }


}
