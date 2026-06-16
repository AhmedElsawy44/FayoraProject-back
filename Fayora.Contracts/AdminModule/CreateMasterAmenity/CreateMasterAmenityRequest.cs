namespace Fayora.Contracts.AdminModule.CreateMasterAmenity;

public record CreateMasterAmenityRequest(
    string Name,
    AmenityCategory Category,
    string? IconUrl);

public enum AmenityCategory
{
    General,
    Room,
    Bathroom,
    Kitchen,
    Entertainment,
    Outdoor,
    Accessibility,
    Safety
}