using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Domain.Entities.SharedModule
{
    public abstract class TourPackageBase : AuditableEntity<Guid>
    {
        public string Title { get; protected set; } = null!;
        public string Description { get; protected set; } = null!;
        public TourType TourTypes { get; protected set; }
        public int DurationHours { get; protected set; }
        public int MaxCapacity { get; protected set; }
        public int BookingsCount { get; protected set; }
        public int AvailableSpots => MaxCapacity - BookingsCount;
        public decimal AdultPrice { get; protected set; }
        public decimal ChildPrice { get; protected set; }
        public bool IsActive { get; protected set; }
        public int Views { get; protected set; }
        public string? MainImageUrl { get; protected set; }
        public string? MainVideoUrl { get; protected set; }
        public string? GuestRequirements { get; protected set; }

        private readonly List<string> _includedItems = [];
        public IReadOnlyCollection<string> IncludedItems => _includedItems.AsReadOnly();

        private readonly List<string> _excludedItems = [];
        public IReadOnlyCollection<string> ExcludedItems => _excludedItems.AsReadOnly();

        protected TourPackageBase() { }

        protected TourPackageBase(
            string title,
            string description,
            TourType tourTypes,
            int durationHours,
            int maxCapacity,
            decimal adultPrice,
            decimal childPrice,
            string? mainImageUrl,
            string? mainVideoUrl,
            string? guestRequirements)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            TourTypes = tourTypes;
            DurationHours = durationHours;
            MaxCapacity = maxCapacity;
            AdultPrice = adultPrice;
            ChildPrice = childPrice;
            MainImageUrl = mainImageUrl;
            MainVideoUrl = mainVideoUrl;
            GuestRequirements = guestRequirements;
            IsActive = false;
            Views = 0;
            BookingsCount = 0;
        }

        public void AddIncludedItem(string item) => _includedItems.Add(item);
        public void AddExcludedItem(string item) => _excludedItems.Add(item);
        public void AddIncludedItems(IEnumerable<string> items) => _includedItems.AddRange(items);
        public void AddExcludedItems(IEnumerable<string> items) => _excludedItems.AddRange(items);

        public void Activate()
        {
            IsActive = true;
            Updated();
        }

        public void Deactivate()
        {
            IsActive = false;
            Updated();
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

        public Result<Success> DecrementBookingsCount()
        {
            if (BookingsCount <= 0)
                return Error.Validation("Package.InvalidCount", "Bookings count cannot be negative.");
            BookingsCount--;
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

        public void SetMainImage(string imageUrl)
        {
            MainImageUrl = imageUrl;
            Updated();
        }


    }
}