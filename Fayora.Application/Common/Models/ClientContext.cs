namespace Fayora.Application.Common.Models;

public record ClientContext(
    Guid UserId,
    string IpAddress,
    string DeviceId,
    string Name,
    string Email,
    string PhoneNumber,
    string AvatarUrl,
    IEnumerable<string> Roles,
    Guid? OwnerId = null,
    Guid? TouristId = null,
    Guid? TourGuideId = null);