using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.SharedModule
{
    public class LocationImage
    {
        public Guid Id { get; init; }
        public int LocationId { get; init; }
        public FileUrl ImageUrl { get; init; } = null!;

        public LocationImage(int locationId, FileUrl imageUrl)
        {
            Id = Guid.NewGuid();
            LocationId = locationId;
            ImageUrl = imageUrl;
        }

        private LocationImage() { }
    }
}
