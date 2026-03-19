using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.TourGuide
{

    public class GuideTourPackageImage
    {
        public Guid Id { get; private set; }
        public Guid PackageId { get; private set; }
        public string ImageUrl { get; private set; } = null!;

        public GuideTourPackageImage(Guid packageId, string imageUrl)
        {
            Id = Guid.NewGuid();
            PackageId = packageId;
            ImageUrl = imageUrl;
        }

        private GuideTourPackageImage() { }
    }
}
