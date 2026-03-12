namespace Fayora.Domain.Entitties.Tourist;

public class MasterInterest : BaseEntity<int>
{
    public string Code { get; init; } = default!;
    public string Name { get; private set; } = default!;
    public string IconUrl { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreateAt { get; init; }

    public MasterInterest(string code, string name, string iconUrl, int sortOrder)
    {
        Code = code;
        Name = name;
        IconUrl = iconUrl;
        SortOrder = sortOrder;
        CreateAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    // don't use this method for normal creation, only for seeding data with specific Ids and CreateAt
    public static MasterInterest CreateForSeed(int id, string code, string name, string iconUrl, int sortOrder)
    {
        return new MasterInterest
        {
            Id = id,
            Code = code,
            Name = name,
            IconUrl = iconUrl,
            SortOrder = sortOrder,
            CreateAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string iconUrl, int sortOrder)
    {
        Name = name;
        IconUrl = iconUrl;
        SortOrder = sortOrder;
    }

    private MasterInterest() { }
}
