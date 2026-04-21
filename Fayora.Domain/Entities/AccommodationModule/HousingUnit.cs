using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.AccommodationModule;

public class HousingUnit : BaseEntity<Guid>
{
    public Guid OwnerId { get; init; }

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public HousingType Type { get; private set; }

    public int LocationId { get; private set; }
    public string AddressDetails { get; private set; }
    public GeoPoint Coordinates { get; private set; }

    public int NumberOfRooms { get; private set; }
    public int BedRooms { get; private set; }
    public int BathRooms { get; private set; }
    public int NumberOfBeds { get; private set; }
    public int MaxGuests { get; private set; }

    public TimeSpan CheckInTime { get; private set; }
    public TimeSpan CheckOutTime { get; private set; }

    public decimal PricePerNight { get; private set; }
    public decimal CommissionRate { get; private set; }

    public AccommodationStatus Status { get; private set; }
    public decimal Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public int Views { get; private set; }

    public string? MainImageUrl { get; private set; }

    private readonly List<HousingUnitImage> _images = [];
    public IReadOnlyCollection<HousingUnitImage> Images => _images.AsReadOnly();

    private readonly List<UnitAmenity> _amenities = [];
    public IReadOnlyCollection<UnitAmenity> Amenities => _amenities.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public HousingUnit(
        Guid ownerId,
        string title,
        string? description,
        int locationId,
        string addressDetails,
        GeoPoint coordinates,
        HousingType type,
        decimal pricePerNight,
        int numberOfRooms,
        int bedRooms,
        int bathRooms,
        int numberOfBeds,
        int maxGuests,
        TimeSpan checkInTime,
        TimeSpan checkOutTime,
        string? mainImageUrl)
    {
        OwnerId = ownerId;
        Title = title;
        Description = description;
        LocationId = locationId;
        AddressDetails = addressDetails;
        Coordinates = coordinates;
        Type = type;
        PricePerNight = pricePerNight;
        NumberOfRooms = numberOfRooms;
        BedRooms = bedRooms;
        BathRooms = bathRooms;
        NumberOfBeds = numberOfBeds;
        MaxGuests = maxGuests;
        CheckInTime = checkInTime;
        CheckOutTime = checkOutTime;
        MainImageUrl = mainImageUrl;

        Status = AccommodationStatus.Pending;
        Rating = 0m;
        ReviewCount = 0;
        Views = 0;
    }

    public void Approve(decimal commissionRate)
    {
        if (commissionRate < 0)
            throw new ArgumentException("Commission rate cannot be negative.", nameof(commissionRate));

        CommissionRate = commissionRate;
        Status = AccommodationStatus.Active;
    }
    public void Reject() => Status = AccommodationStatus.Rejected;
    public void Deactivate() => Status = AccommodationStatus.Inactive;

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice > 0) PricePerNight = newPrice;
    }

    public void IncrementViews() => Views++;

    public void AddImage(HousingUnitImage image) => _images.Add(image);

    public void AddImages(IEnumerable<HousingUnitImage> images) => _images.AddRange(images);

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is not null) _images.Remove(image);
    }

    public void AddAmenity(int amenityId)
    {
        if (!_amenities.Any(a => a.AmenityId == amenityId))
            _amenities.Add(new UnitAmenity(this.Id, amenityId));
    }

    public void AddAmenities(IEnumerable<int> amenityIds)
    {
        foreach (var amenityId in amenityIds)
            AddAmenity(amenityId);
    }

    public void RemoveAmenity(int amenityId)
    {
        var amenity = _amenities.FirstOrDefault(a => a.AmenityId == amenityId);
        if (amenity is not null) _amenities.Remove(amenity);
    }

    private HousingUnit() { }
}