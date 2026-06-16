using System;

namespace Fayora.Domain.Enums.TourGuideModule
{
    [Flags]
    public enum PackageMeals
    {
        None = 0,
        Breakfast = 1,
        Lunch = 2,
        Dinner = 4
    }
}
