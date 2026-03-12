namespace Fayora.Domain.Entitties.Accommodation;

public class MasterAmenity : BaseEntity<int>
{
    public string Name { get; private set; } = default!;
    public string IconUrl { get; private set; } = default!;
    public string Category { get; private set; } = default!;
}
