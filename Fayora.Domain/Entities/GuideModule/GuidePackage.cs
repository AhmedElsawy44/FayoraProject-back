using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class GuidePackage : AuditableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TourType TourTypes { get; private set; }
    public int DurationHours { get; private set; }
    public int MaxCapacity { get; private set; }
    public int BookingsCount { get; private set; }
    public int AvailableSpots => MaxCapacity - BookingsCount;
    public decimal AdultPrice { get; private set; }
    public decimal ChildPrice { get; private set; }
    public bool IsActive { get; private set; }
    public int Views { get; private set; }
    public FileUrl MainImageUrl { get; private set; }
    public FileUrl? MainVideoUrl { get; private set; }
    public string? GuestRequirements { get; private set; }
    public CancellationPolicy CancellationPolicy { get; private set; }
    public ItemStatus PackageStatus { get; private set; }

    private readonly List<int> _includedItemIds = [];
    public IReadOnlyCollection<int> IncludedItemIds => _includedItemIds.AsReadOnly();

    private readonly List<int> _excludedItemIds = [];
    public IReadOnlyCollection<int> ExcludedItemIds => _excludedItemIds.AsReadOnly();
    public GeoPoint MeetingPoint { get; private set; } = null!;
    public string? ArrivalNote { get; private set; }
    public TransportType TransportType { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private readonly List<Guid> _imageIds = [];
    public IReadOnlyCollection<Guid> ImageIds => _imageIds.AsReadOnly();
    public IReadOnlyCollection<Guid> ImageURLs => _imageIds.ToList().AsReadOnly();

    private readonly List<Guid> _activityIds = [];
    public IReadOnlyCollection<Guid> ActivityIds => _activityIds.AsReadOnly();

    private GuidePackage() { }

    private GuidePackage(
        Guid guideId,
        string title,
        string description,
        TourType tourTypes,
        int durationHours,
        GeoPoint meetingPoint,
        TransportType transportType,
        int maxCapacity,
        decimal adultPrice,
        decimal childPrice,
        string? arrivalNote,
        FileUrl mainImageUrl,
        FileUrl? mainVideoUrl,
        string? guestRequirements,
        CancellationPolicy cancellationPolicy)
    {
        Id = Guid.NewGuid();
        UserId = guideId;
        Title = title;
        Description = description;
        TourTypes = tourTypes;
        DurationHours = durationHours;
        MeetingPoint = meetingPoint;
        TransportType = transportType;
        MaxCapacity = maxCapacity;
        AdultPrice = adultPrice;
        ChildPrice = childPrice;
        ArrivalNote = arrivalNote;
        MainImageUrl = mainImageUrl;
        MainVideoUrl = mainVideoUrl;
        GuestRequirements = guestRequirements;

        IsActive = false;
        Views = 0;
        BookingsCount = 0;

        CancellationPolicy = cancellationPolicy;
        PackageStatus = ItemStatus.Pending;
    }

    public static Result<GuidePackage> Create(
        Guid guideId, string title, string description,
        TourType tourTypes, int durationHours,
        GeoPoint meetingPoint, TransportType transportType,
        int maxCapacity, decimal adultPrice, decimal childPrice,
        string? arrivalNote, FileUrl mainImageUrl,
        FileUrl? mainVideoUrl = null, string? guestRequirements = null, CancellationPolicy cancellationPolicy = CancellationPolicy.NonRefundable)
    {
        if (adultPrice <= 0)
            return Error.Validation("Package.InvalidPrice", "Adult price must be positive.");

        if (durationHours <= 0)
            return Error.Validation("Package.InvalidDuration", "Duration must be greater than zero.");

        if (maxCapacity <= 0)
            return Error.Validation("Package.InvalidCapacity", "Max capacity must be greater than zero.");

        return new GuidePackage(guideId, title, description, tourTypes,
            durationHours, meetingPoint, transportType, maxCapacity,
            adultPrice, childPrice, arrivalNote, mainImageUrl,
            mainVideoUrl, guestRequirements, cancellationPolicy);
    }

    public void AddIncludedItem(int id) => _includedItemIds.Add(id);
    public void AddExcludedItem(int id) => _excludedItemIds.Add(id);
    public void AddImage(Guid imageId)
    {
        _imageIds.Add(imageId);
    }

    public void AddImages(IEnumerable<Guid> imageIds)
    {
        foreach (var id in imageIds) AddImage(id);
    }

    public Result<Success> Activate()
    {
        if (IsActive)
            return Error.Conflict("Package.AlreadyActive", "Package is already active.");
        IsActive = true;
        Updated();
        return Result.Success;
    }

    public Result<Success> Deactivate()
    {
        if (!IsActive)
            return Error.Conflict("Package.AlreadyInactive", "Package is already inactive.");

        IsActive = false;
        Updated();
        return Result.Success;
    }

    public void IncrementViews() => Views++;

    public Result<Success> IncrementBookingsCount()
    {
        if (BookingsCount >= MaxCapacity)
            return Error.Conflict("Package.FullyBooked", "Package is fully booked.");
        BookingsCount++;
        Updated();
        return Result.Success;
    }

    public void UpdateDetails(
        string title,
        string description,
        int durationHours,
        decimal adultPrice,
        decimal childPrice,
        TourType tourTypes)
    {
        Title = title;
        Description = description;
        DurationHours = durationHours;
        AdultPrice = adultPrice;
        ChildPrice = childPrice;
        TourTypes = tourTypes;
        Updated();
    }

    public void AddIncludedItems(IEnumerable<int> ids)
    {
        foreach (var id in ids) _includedItemIds.Add(id);
    }

    public void AddExcludedItems(IEnumerable<int> ids)
    {
        foreach (var id in ids) _excludedItemIds.Add(id);
    }

    public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
    {
        MeetingPoint = newMeetingPoint;
        Updated();
    }

    public void AddActivities(IEnumerable<Guid> activityIds)
    {
        foreach (var id in activityIds) _activityIds.Add(id);
    }

    public void Delete() => DeletedAt = DateTimeOffset.UtcNow;
}