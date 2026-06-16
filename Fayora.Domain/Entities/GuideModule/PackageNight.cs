using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.GuideModule
{
    public class PackageNight : BaseEntity<Guid>
    {
        public Guid PackageId { get; private set; }
        public int NightNumber { get; private set; }
        public DateOnly NightDate { get; private set; }
        public Guid? HousingUnitId { get; private set; }
        public Guid? PackageAccommodationId { get; private set; }

        public static Result<PackageNight> Create(
            Guid packageId,
            int nightNumber,
            DateOnly nightDate,
            Guid? housingUnitId,
            Guid? packageAccommodationId)
        {
            if (housingUnitId.HasValue && packageAccommodationId.HasValue)
                return Error.Validation("PackageNight.BothAccommodations",
                    "Cannot have both a housing unit and a package accommodation.");

            if (!housingUnitId.HasValue && !packageAccommodationId.HasValue)
                return Error.Validation("PackageNight.NoAccommodation",
                    "Must have either a housing unit or a package accommodation.");

            if (nightNumber <= 0)
                return Error.Validation("PackageNight.InvalidNightNumber",
                    "Night number must be greater than zero.");

            return new PackageNight
            {
                Id = Guid.NewGuid(),
                PackageId = packageId,
                NightNumber = nightNumber,
                NightDate = nightDate,
                HousingUnitId = housingUnitId,
                PackageAccommodationId = packageAccommodationId
            };
        }

        private PackageNight() { }
    }
}
