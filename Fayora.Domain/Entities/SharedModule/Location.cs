using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.SharedModule
{
    public class Location : BaseEntity<int>
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public GeoPoint Coordinates { get; private set; } = null!;
        public decimal Rating { get; private set; }
        public int ReviewCount { get; private set; }
        public FileUrl MainImageUrl { get; private set; } = null!;

        private readonly List<Guid> _imageIds = [];
        public IReadOnlyCollection<Guid> ImageIds => _imageIds.AsReadOnly();

        public static Result<Location> Create(
            string name,
            string? description,
            decimal latitude,
            decimal longitude,
            FileUrl mainImageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Error.Validation("Location.Name", "Name is required.");

            var coordinates = GeoPoint.Create(latitude, longitude);
            if (coordinates.IsError) return coordinates.Errors;

            return new Location
            {
                Name = name,
                Description = description,
                Coordinates = coordinates.Value,
                MainImageUrl = mainImageUrl,
                Rating = 0,
                ReviewCount = 0
            };
        }

        public void AddImage(Guid imageId) => _imageIds.Add(imageId);
        public void AddImages(IEnumerable<Guid> imageIds) => _imageIds.AddRange(imageIds);

        private Location() { }
    }
}
