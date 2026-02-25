using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.Identity;

public class RefreshToken
{
    public Guid UserId { get; set; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; private set; }
    public string DeviceId { get; init; } = string.Empty;
    public string? IpAddress { get; init; }

    public RefreshToken(string token, string deviceId, string? ipAddress)
    {
        Token = token;
        DeviceId = deviceId;
        IpAddress = ipAddress;
        ExpiresAt = DateTime.UtcNow.AddDays(7);

    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    private RefreshToken() { }
}   
