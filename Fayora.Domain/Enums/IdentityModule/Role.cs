namespace Fayora.Domain.Enums.IdentityModule;

[Flags]
public enum Role
{
    Admin = 1,
    Tourist = 2,
    TourGuide = 4,
    TourCompany = 8,
    Support = 16
}
