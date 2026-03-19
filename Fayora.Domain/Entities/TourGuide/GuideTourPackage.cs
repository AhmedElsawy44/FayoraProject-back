using Fayora.Domain.Common.Entity;
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


        public static GuideTourPackage Create(
            Guid guideId,
            string title,
            string description,
            int durationHours,
            GeoPoint meetingPoint,
            TransportType transportType,
            int maxCapacity,
            decimal pricePerPerson,
            string? mainImageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Title cannot be empty.");

            if (durationHours <= 0)
                throw new InvalidOperationException("Duration must be greater than zero.");

            if (pricePerPerson <= 0)
                throw new InvalidOperationException("Price must be greater than zero.");

            if (maxCapacity <= 0)
                throw new InvalidOperationException("Max capacity must be greater than zero.");

            return new GuideTourPackage(guideId, title, description, durationHours,
                meetingPoint, transportType, maxCapacity,
                pricePerPerson, mainImageUrl);
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

        public void IncrementBookingsCount()
        {
            if (BookingsCount >= MaxCapacity)
                throw new InvalidOperationException("Package is fully booked.");
            BookingsCount++;
        }

        public void DecrementBookingsCount()
        {
            if (BookingsCount <= 0)
                throw new InvalidOperationException("Bookings count cannot be negative.");
            BookingsCount--;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Package is already active.");
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Package is already inactive.");
            IsActive = false;
        }
        public void IncrementViews() => Views++;

        public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            MeetingPoint = newMeetingPoint;
            Updated();
        }

        public void UpdateMainImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new InvalidOperationException("Image URL cannot be empty.");

            MainImageUrl = imageUrl;
            Updated();
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image is not null)
                _images.Remove(image);
        }



    }

}
