namespace Fayora.Domain.Enums.AccommodationModule;

[Flags]
public enum Amenities
{
    Wifi = 1,
    AirConditioning = 2,
    Heating = 4,
    Kitchen = 8,
    Parking = 16,
    Pool = 32,
    Gym = 64,
    PetFriendly = 128,
    Washer = 256,
    Dryer = 512,
}
