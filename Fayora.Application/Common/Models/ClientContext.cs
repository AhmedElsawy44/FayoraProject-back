namespace Fayora.Application.Common.Models;

public record ClientContext(Guid UserId, string IpAddress, string? DeviceId, IEnumerable<string> Roles);
