namespace Fayora.Domain.Entitties.Tourist;

public class MasterInterest : BaseEntity<int>
{
    public string Code { get; init; } = default!;
    public string Name { get; private set; } = default!;
    public string IconUrl { get; private set; } = default!;
    public DateTimeOffset CreateAt { get; init; }

    public MasterInterest(string code, string name, string iconUrl)
    {
        Code = code;
        Name = name;
        IconUrl = iconUrl;
        CreateAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(string name, string iconUrl)
    {
        Name = name;
        IconUrl = iconUrl;
    }

    private MasterInterest() { }
}
