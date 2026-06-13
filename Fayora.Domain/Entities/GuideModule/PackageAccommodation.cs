using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.GuideModule
{
    public class PackageAccommodation : BaseEntity<Guid>
    {
        public Guid PackageId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public PackageAccommodationType Type { get; private set; }
        public FileUrl MainImageUrl { get; private set; } = null!;
        public GeoPoint Location { get; private set; } = null!;
        public TimeOnly CheckInTime { get; private set; }
        public TimeOnly CheckOutTime { get; private set; }
        public PackageAmenities Amenities { get; private set; } = PackageAmenities.None;
        public PackageMeals Meals { get; private set; } = PackageMeals.None;

        private readonly List<FileUrl> _galleryImages = new();
        public IReadOnlyList<FileUrl> GalleryImages => _galleryImages.AsReadOnly();

        public static Result<PackageAccommodation> Create(
            Guid packageId,
            string name,
            string? description,
            PackageAccommodationType type,
            string mainImageUrl,
            decimal latitude,
            decimal longitude,
            TimeOnly checkInTime,
            TimeOnly checkOutTime,
            PackageAmenities amenities = PackageAmenities.None,
            PackageMeals meals = PackageMeals.None,
            List<string>? galleryImages = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Error.Validation("PackageAccommodation.Name", "Name is required.");

            var mainImageResult = FileUrl.Create(mainImageUrl);
            if (mainImageResult.IsError) return mainImageResult.Errors;

            var locationResult = GeoPoint.Create(latitude, longitude);
            if (locationResult.IsError) return locationResult.Errors;

            if (checkInTime >= checkOutTime)
                return Error.Validation("PackageAccommodation.CheckInTime",
                    "Check-in time must be before check-out time.");

            var galleryUrls = new List<FileUrl>();
            if (galleryImages?.Any() == true)
            {
                foreach (var url in galleryImages)
                {
                    var urlResult = FileUrl.Create(url);
                    if (urlResult.IsError) return urlResult.Errors;
                    galleryUrls.Add(urlResult.Value);
                }
            }


            var accommodation = new PackageAccommodation
            {
                Id = Guid.NewGuid(),
                PackageId = packageId,
                Name = name,
                Description = description,
                Type = type,
                MainImageUrl = mainImageResult.Value,
                Location = locationResult.Value,
                CheckInTime = checkInTime,
                CheckOutTime = checkOutTime,
                Amenities = amenities,
                Meals = meals
            };

            accommodation._galleryImages.AddRange(galleryUrls);
            return accommodation;
        }

        private PackageAccommodation() { }
    }
}
