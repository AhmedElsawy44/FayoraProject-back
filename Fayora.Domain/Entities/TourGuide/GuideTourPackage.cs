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
        public int DurationHours { get; private set; }
        public GeoPoint MeetingPoint { get; private set; } = null!;
        public TransportType TransportType { get; private set; }
        public int MaxCapacity { get; private set; }
        public int BookingsCount { get; private set; }
        public int AvailableSpots => MaxCapacity - BookingsCount;
        public decimal PricePerPerson { get; private set; }
        public bool IsActive { get; private set; }
        public int Views { get; private set; }
        public string? MainImageUrl { get; private set; }

        private readonly List<GuideTourPackageImage> _images = [];
        public IReadOnlyCollection<GuideTourPackageImage> Images => _images.AsReadOnly();


        private readonly List<string> _includedItems = [];
        public IReadOnlyCollection<string> IncludedItems => _includedItems.AsReadOnly();

        private readonly List<string> _excludedItems = [];
        public IReadOnlyCollection<string> ExcludedItems => _excludedItems.AsReadOnly();


        private GuideTourPackage(Guid guideId,
            string title,
            string description,
            int durationHours,
            GeoPoint meetingPoint,
            TransportType transportType,
            int maxCapacity,
            decimal pricePerPerson,
            string? mainImageUrl = null
            )
        {
            GuideId = guideId;
            Title = title;
            Description = description;
            DurationHours = durationHours;
            MeetingPoint = meetingPoint;
            TransportType = transportType;
            MaxCapacity = maxCapacity;
            PricePerPerson = pricePerPerson;
            MainImageUrl = mainImageUrl;
            IsActive = false;
            Views = 0;
            BookingsCount = 0;
        }

        private GuideTourPackage() { }



        public static Result<GuideTourPackage> Create(
           Guid guideId, string title, string description,
           int durationHours, GeoPoint meetingPoint,
           TransportType transportType, int maxCapacity,
           decimal pricePerPerson, string? mainImageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("Package.EmptyTitle", "Title cannot be empty.");

            if (durationHours <= 0)
                return Error.Validation("Package.InvalidDuration", "Duration must be greater than zero.");

            if (pricePerPerson <= 0)
                return Error.Validation("Package.InvalidPrice", "Price must be greater than zero.");

            if (maxCapacity <= 0)
                return Error.Validation("Package.InvalidCapacity", "Max capacity must be greater than zero.");

            return new GuideTourPackage(guideId, title, description, durationHours,
                meetingPoint, transportType, maxCapacity, pricePerPerson, mainImageUrl);
        }

        public void AddImage(string imageUrl)
    => _images.Add(new GuideTourPackageImage(Id, imageUrl));

        public void AddIncludedItem(string item)
            => _includedItems.Add(item);

        public void AddExcludedItem(string item)
            => _excludedItems.Add(item);

        public void ClearIncludedItems() => _includedItems.Clear();
        public void ClearExcludedItems() => _excludedItems.Clear();

        public void UpdateDetails(string title, string description, int durationHours, decimal pricePerPerson)
        {
            Title = title;
            Description = description;
            DurationHours = durationHours;
            PricePerPerson = pricePerPerson;
            Updated();
        }

        public Result<Success> IncrementBookingsCount()
        {
            if (BookingsCount >= MaxCapacity)
                return Error.Conflict("Package.FullyBooked", "Package is fully booked.");
            BookingsCount++;
            return Result.Success;
        }

        public Result<Success> DecrementBookingsCount()
        {
            if (BookingsCount <= 0)
                return Error.Validation("Package.InvalidCount", "Bookings count cannot be negative.");
            BookingsCount--;
            return Result.Success;
        }

        public Result<Success> Activate()
        {
            if (IsActive)
                return Error.Conflict("Package.AlreadyActive", "Package is already active.");
            IsActive = true;
            return Result.Success;
        }

        public Result<Success> Deactivate()
        {
            if (!IsActive)
                return Error.Conflict("Package.AlreadyInactive", "Package is already inactive.");
            IsActive = false;
            return Result.Success;
        }
        public void IncrementViews() => Views++;

        public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            MeetingPoint = newMeetingPoint;
            Updated();
        }

        public Result<Success> UpdateMainImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Error.Validation("Package.InvalidImage", "Image URL cannot be empty.");
            MainImageUrl = imageUrl;
            Updated();
            return Result.Success;
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image is not null)
                _images.Remove(image);
        }



    }

}
