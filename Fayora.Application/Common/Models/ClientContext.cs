namespace Fayora.Application.Common.Models;

public record ClientContext(
    Guid UserId,
    string IpAddress,
    string DeviceId,
    string UserName,
    string Email,
    string PhoneNumber,
    string UserAvatarUrl,
    IEnumerable<string> Roles,
    Guid? OwnerId = null,
    Guid? TouristId = null,
    Guid? TourGuideId = null);