using Fayora.Domain.Common.Results;
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

    public FileUrl MainImageUrl { get; private set; } = null!;

    private readonly List<Guid> _imageIds = [];
    public IReadOnlyCollection<Guid> ImageIds => _imageIds.AsReadOnly();

    private readonly List<int> _amenities = [];
    public IReadOnlyCollection<int> Amenities => _amenities.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private HousingUnit(
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
        FileUrl mainImageUrl)
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

    public static Result<HousingUnit> Create(
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
    FileUrl mainImageUrl)
    {
        if (ownerId == Guid.Empty)
            return Error.Validation("HousingUnit.OwnerId", "Owner ID is required.");

        if (string.IsNullOrWhiteSpace(title))
            return Error.Validation("HousingUnit.Title", "Title is required.");

        if (title.Length > 100)
            return Error.Validation("HousingUnit.Title", "Title must not exceed 100 characters.");

        if (pricePerNight <= 0)
            return Error.Validation("HousingUnit.PricePerNight", "Price per night must be greater than zero.");

        if (numberOfRooms <= 0)
            return Error.Validation("HousingUnit.NumberOfRooms", "Number of rooms must be greater than zero.");

        if (bedRooms < 0)
            return Error.Validation("HousingUnit.BedRooms", "Bedrooms cannot be negative.");

        if (bathRooms < 0)
            return Error.Validation("HousingUnit.BathRooms", "Bathrooms cannot be negative.");

        if (numberOfBeds <= 0)
            return Error.Validation("HousingUnit.NumberOfBeds", "Number of beds must be greater than zero.");

        if (maxGuests <= 0)
            return Error.Validation("HousingUnit.MaxGuests", "Max guests must be greater than zero.");

        if (checkInTime >= checkOutTime)
            return Error.Validation("HousingUnit.CheckInTime", "Check-in time must be before check-out time.");

        if (string.IsNullOrWhiteSpace(addressDetails))
            return Error.Validation("HousingUnit.AddressDetails", "Address details are required.");

        return new HousingUnit(
            ownerId,
            title,
            description,
            locationId,
            addressDetails,
            coordinates,
            type,
            pricePerNight,
            numberOfRooms,
            bedRooms,
            bathRooms,
            numberOfBeds,
            maxGuests,
            checkInTime,
            checkOutTime,
            mainImageUrl);
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

    public void AddImage(Guid id) => _imageIds.Add(id);

    public void AddImages(IEnumerable<Guid> ids) => _imageIds.AddRange(ids);

    public void RemoveImage(Guid imageId)
    {
        _imageIds.Remove(imageId);
    }

    public void AddAmenity(int amenityId)
    {
        if (!_amenities.Any(a => a == amenityId))
            _amenities.Add(amenityId);
    }

    public void AddAmenities(IEnumerable<int> amenityIds)
    {
        if (_amenities.All(a => !amenityIds.Contains(a)))
            _amenities.AddRange(amenityIds);
    }

    public void RemoveAmenity(int amenityId)
    {
        _amenities.Remove(amenityId);
    }

    private HousingUnit() { }
}