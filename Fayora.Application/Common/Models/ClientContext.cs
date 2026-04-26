namespace Fayora.Application.Common.Models;

public record ClientContext(
    Guid UserId,
    string IpAddress,
    string DeviceId,
    string UserName,
    string Email,
    string PhoneNumber,
    string AvatarUrl,
    IEnumerable<string> Roles);