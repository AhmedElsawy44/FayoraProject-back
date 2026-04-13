using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.ValueObjects;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Domain.Entities.TourCompanyModule
{
    public class CompanyTourPackage : TourPackageBase
    {
        public Guid CompanyId { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public GeoPoint DepartureLocation { get; private set; } = null!;
        public string? CancellationPolicy { get; private set; }

        private readonly List<CompanyTourPackageImage> _images = [];
        public IReadOnlyCollection<CompanyTourPackageImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<string> ImageURLs => _images.Select(i => i.ImageUrl).ToList().AsReadOnly();

        private readonly List<PackageActivity> _activities = [];
        public IReadOnlyCollection<PackageActivity> Activities => _activities.AsReadOnly();

        public TourCompany Company { get; private set; } = null!; // Navigation property

        private CompanyTourPackage(
            Guid companyId, string title, string description,
            TourType tourTypes, int durationHours,
            DateOnly startDate, DateOnly endDate,
            GeoPoint departureLocation, int maxCapacity,
            decimal adultPrice, decimal childPrice,
            string? cancellationPolicy, string? mainImageUrl,
            string? mainVideoUrl, string? guestRequirements)
            : base(title, description, tourTypes, durationHours, maxCapacity,
                  adultPrice, childPrice, mainImageUrl, mainVideoUrl, guestRequirements)
        {
            CompanyId = companyId;
            StartDate = startDate;
            EndDate = endDate;
            DepartureLocation = departureLocation;
            CancellationPolicy = cancellationPolicy;
        }

        private CompanyTourPackage() { }

        public static Result<CompanyTourPackage> Create(
            Guid companyId, string title, string description,
            TourType tourTypes, int durationHours,
            DateOnly startDate, DateOnly endDate,
            GeoPoint departureLocation, int maxCapacity,
            decimal adultPrice, decimal childPrice,
            string? cancellationPolicy = null, string? mainImageUrl = null,
            string? mainVideoUrl = null, string? guestRequirements = null)
        {
            if (adultPrice <= 0)
                return Error.Validation("Package.InvalidPrice", "Adult price must be positive.");

            if (durationHours <= 0)
                return Error.Validation("Package.InvalidDuration", "Duration must be greater than zero.");

            if (maxCapacity <= 0)
                return Error.Validation("Package.InvalidCapacity", "Max capacity must be greater than zero.");

            if (startDate >= endDate)
                return Error.Validation("Package.InvalidDates", "Start date must be before end date.");

            if (startDate < DateOnly.FromDateTime(DateTime.UtcNow))
                return Error.Validation("Package.PastDate", "Start date cannot be in the past.");

            return new CompanyTourPackage(companyId, title, description, tourTypes,
                durationHours, startDate, endDate, departureLocation, maxCapacity,
                adultPrice, childPrice, cancellationPolicy, mainImageUrl,
                mainVideoUrl, guestRequirements);
        }

        public void AddImage(string imageUrl)
            => _images.Add(new CompanyTourPackageImage(Id, imageUrl));

        public void AddActivity(PackageActivity activity)
            => _activities.Add(activity);

        public void UpdateDates(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            Updated();
        }

        public void UpdateDepartureLocation(GeoPoint newLocation)
        {
            DepartureLocation = newLocation;
            Updated();
        }
    }
}
