namespace Fayora.Application.Common.Models;

public record ClientContext(
    Guid UserId,
    string IpAddress,
    string DeviceId,
    string Email,
    string PhoneNumber,
    IEnumerable<string> Roles,
    Guid? OwnerId = null,
    Guid? TouristId = null,
    Guid? TourGuideId = null);