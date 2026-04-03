using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TourGuide
{
    public class GuideTourPackage : AuditableEntity<Guid>
    {
        public Guid GuideId { get; private set; }
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public TourType TourType { get; private set; }
        public int DurationHours { get; private set; }
        public GeoPoint MeetingPoint { get; private set; } = null!;
        public string? ArrivalNote { get; private set; }

        public TransportType TransportType { get; private set; }
        public int MaxCapacity { get; private set; }
        public int BookingsCount { get; private set; }
        public int AvailableSpots => MaxCapacity - BookingsCount;
        public decimal AdultPrice { get; private set; }
        public decimal ChildPrice { get; private set; }
        public bool IsActive { get; private set; }
        public int Views { get; private set; }
        public string? MainImageUrl { get; private set; }
        public string? MainVideoUrl { get; private set; }
        public string? GuestRequirements { get; private set; }


        private readonly List<GuideTourPackageImage> _images = [];
        public IReadOnlyCollection<GuideTourPackageImage> Images => _images.AsReadOnly();

        private readonly List<string> _includedItems = [];
        public IReadOnlyCollection<string> IncludedItems => _includedItems.AsReadOnly();

        private readonly List<string> _excludedItems = [];
        public IReadOnlyCollection<string> ExcludedItems => _excludedItems.AsReadOnly();

        private GuideTourPackage(
        Guid guideId, string title, string description, TourType tourType,
        int durationHours, GeoPoint meetingPoint, TransportType transportType,
        int maxCapacity, decimal adultPrice, decimal childPrice,
        string? arrivalNote, string? mainImageUrl, string? mainVideoUrl, string? guestRequirements)
        {
            Id = Guid.NewGuid();
            GuideId = guideId;
            Title = title;
            Description = description;
            TourType = tourType;
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
        }

        public static Result<GuideTourPackage> Create(
            Guid guideId, string title, string description, TourType tourType,
            int durationHours, GeoPoint meetingPoint, TransportType transportType,
            int maxCapacity, decimal adultPrice, decimal childPrice,
            string? arrivalNote, string? mainImageUrl, string? mainVideoUrl, string? guestRequirements)
        {
            if (adultPrice <= 0) return Error.Validation("Package.InvalidPrice", "Adult price must be positive.");

            var package = new GuideTourPackage(guideId, title, description, tourType,
                durationHours, meetingPoint, transportType, maxCapacity, adultPrice,
                childPrice, arrivalNote, mainImageUrl, mainVideoUrl, guestRequirements);

            return package;
        }

        public void AddIncludedItems(IEnumerable<string> items) => _includedItems.AddRange(items);
        public void AddExcludedItems(IEnumerable<string> items) => _excludedItems.AddRange(items);

        private GuideTourPackage() { }

        public void AddImage(string imageUrl)
            => _images.Add(new GuideTourPackageImage(Id, imageUrl));

        public void AddIncludedItem(string item) => _includedItems.Add(item);
        public void AddExcludedItem(string item) => _excludedItems.Add(item);

        public void UpdateDetails(string title, string description, int durationHours, decimal adultPrice)
        {
            Title = title;
            Description = description;
            DurationHours = durationHours;
            AdultPrice = adultPrice;
            Updated();
        }

        public Result<Success> IncrementBookingsCount()
        {
            if (BookingsCount >= MaxCapacity)
                return Error.Conflict("Package.FullyBooked", "Package is fully booked.");
            BookingsCount++;
            Updated();
            return Result.Success;
        }

        public Result<Success> DecrementBookingsCount()
        {
            if (BookingsCount <= 0)
                return Error.Validation("Package.InvalidCount", "Bookings count cannot be negative.");
            BookingsCount--;
            Updated();
            return Result.Success;
        }

        public Result<Success> Activate()
        {
            IsActive = true;
            Updated();
            return Result.Success;
        }

        public Result<Success> Deactivate()
        {
            IsActive = false;
            Updated();
            return Result.Success;
        }

        public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            MeetingPoint = newMeetingPoint;
            Updated();
        }

        public void IncrementViews() => Views++;
    }
}