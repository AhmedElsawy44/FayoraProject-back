using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Domain.Entities.TourGuide
{
    public class GuideTourPackage : TourPackageBase
    {
        public Guid GuideId { get; private set; }
        public GeoPoint MeetingPoint { get; private set; } = null!;
        public string? ArrivalNote { get; private set; }
        public TransportType TransportType { get; private set; }

        private readonly List<GuideTourPackageImage> _images = [];
        public IReadOnlyCollection<GuideTourPackageImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<string> ImageURLs => _images.Select(i => i.ImageUrl).ToList().AsReadOnly();

        private GuideTourPackage(
            Guid guideId, string title, string description,
            TourType tourTypes, int durationHours,
            GeoPoint meetingPoint, TransportType transportType,
            int maxCapacity, decimal adultPrice, decimal childPrice,
            string? arrivalNote, string? mainImageUrl,
            string? mainVideoUrl, string? guestRequirements)
            : base(title, description, tourTypes, durationHours, maxCapacity,
                  adultPrice, childPrice, mainImageUrl, mainVideoUrl, guestRequirements)
        {
            GuideId = guideId;
            MeetingPoint = meetingPoint;
            TransportType = transportType;
            ArrivalNote = arrivalNote;
        }

        private GuideTourPackage() { }

        public static Result<GuideTourPackage> Create(
            Guid guideId, string title, string description,
            TourType tourTypes, int durationHours,
            GeoPoint meetingPoint, TransportType transportType,
            int maxCapacity, decimal adultPrice, decimal childPrice,
            string? arrivalNote = null, string? mainImageUrl = null,
            string? mainVideoUrl = null, string? guestRequirements = null)
        {
            if (adultPrice <= 0)
                return Error.Validation("Package.InvalidPrice", "Adult price must be positive.");

            if (durationHours <= 0)
                return Error.Validation("Package.InvalidDuration", "Duration must be greater than zero.");

            if (maxCapacity <= 0)
                return Error.Validation("Package.InvalidCapacity", "Max capacity must be greater than zero.");

            return new GuideTourPackage(guideId, title, description, tourTypes,
                durationHours, meetingPoint, transportType, maxCapacity,
                adultPrice, childPrice, arrivalNote, mainImageUrl,
                mainVideoUrl, guestRequirements);
        }

        public void AddImage(string imageUrl)
            => _images.Add(new GuideTourPackageImage(Id, imageUrl));

        public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            MeetingPoint = newMeetingPoint;
            Updated();
        }
    }
}