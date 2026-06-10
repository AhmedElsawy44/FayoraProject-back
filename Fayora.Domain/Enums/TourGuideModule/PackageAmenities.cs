using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Enums.TourGuideModule
{
    [Flags]
    public enum PackageAmenities
    {
        None = 0,
        Breakfast = 1,
        Lunch = 2,
        Dinner = 4,
        Wifi = 8,
        Pool = 16,
        Parking = 32,
        AirConditioning = 64,
        Gym = 128
    }
}
