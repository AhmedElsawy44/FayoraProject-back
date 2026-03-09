using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Application.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fayora.Api.Services;

public class ClientContextProvider(IHttpContextAccessor accessor) : IClientContextProvider
{
    public ClientContext GetContext()
    {
        var ipAddress = accessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? accessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";

        var deviceId = GetClaimsValue("device_id");

        var userIdString = GetClaimsValue(ClaimTypes.NameIdentifier) ?? GetClaimsValue(JwtRegisteredClaimNames.Sub);
        Guid.TryParse(userIdString, out var userId);

        var roles = GetClaimsValues(ClaimTypes.Role);

        return new ClientContext(userId, ipAddress, deviceId, roles);
    }

    private IEnumerable<string> GetClaimsValues(string claimType)
    {
        return accessor.HttpContext?.User.Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value) ?? Enumerable.Empty<string>();
    }
    private string GetClaimsValue(string claimType)
    {
        return accessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
    }
}
