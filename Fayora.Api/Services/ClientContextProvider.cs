using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Fayora.Api.Services;

public class ClientContextProvider(IHttpContextAccessor accessor) : IClientContextProvider
{
    public ClientContext GetContext()
    {
        var ipAddress = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var deviceId = GetClaimsValue("device_id");

        return new ClientContext(ipAddress, deviceId);
    }

    private string? GetClaimsValue(string claimType)
    {
        return accessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
