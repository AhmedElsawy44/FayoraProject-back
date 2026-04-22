using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.SharedModule
{
    public abstract class TourPackageImageBase
    {
        public Guid Id { get; private set; }
        public Guid PackageId { get; private set; }
        public string ImageUrl { get; private set; } = null!;

        protected TourPackageImageBase(Guid packageId, string imageUrl)
        {
            Id = Guid.NewGuid();
            PackageId = packageId;
            ImageUrl = imageUrl;
        }

        protected TourPackageImageBase() { }
    }
}
