using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Models;

namespace Fayora.Api.Services;

public class ClientContextProvider(IHttpContextAccessor accessor) : IClientContextProvider
{
    public ClientContext GetContext()
    {
        var ipAddress = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var deviceId = GetClaimsValue("device_id");
        Guid.TryParse(GetClaimsValue("user_id"), out var userId);

        return new ClientContext(userId, ipAddress, deviceId);
    }

    private string? GetClaimsValue(string claimType)
    {
        return accessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
