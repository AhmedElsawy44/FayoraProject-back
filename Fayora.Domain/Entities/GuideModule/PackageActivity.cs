namespace Fayora.Domain.Entities.TourGuideModule;

public class PackageActivity : BaseEntity<Guid>
{
    public Guid PackageId { get; private set; }
    public Guid PlaceId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime ActivityDate { get; private set; }
    public int DurationHours { get; private set; }

    public PackageActivity(
        Guid packageId,
        Guid placeId,
        string title,
        string? description,
        DateTime activityDate,
        int durationHours)
    {
        Id = Guid.NewGuid();
        PackageId = packageId;
        PlaceId = placeId;
        Title = title;
        Description = description;
        ActivityDate = activityDate;
        DurationHours = durationHours;
    }

    private PackageActivity() { }
}
