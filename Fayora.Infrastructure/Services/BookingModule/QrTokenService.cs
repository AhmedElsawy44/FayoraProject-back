using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fayora.Infrastructure.Services.BookingModule;

public class QrTokenService(IOptions<JwtSettings> jwtSettings) : IQrTokenService
{
    public string GenerateToken(QrTokenPayload payload)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));

        var claims = new[]
        {
        new Claim("bookingId", payload.BookingId.ToString()),
        new Claim("userId", payload.UserId.ToString()),
        new Claim("serviceProviderId", payload.ServiceProviderId.ToString()),
        new Claim("serviceId", payload.ServiceId.ToString()),
    };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: payload.ExpiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Result<QrTokenPayload> ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));

            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return new QrTokenPayload(
                Guid.Parse(principal.FindFirstValue("bookingId")!),
                Guid.Parse(principal.FindFirstValue("userId")!),
                Guid.Parse(principal.FindFirstValue("serviceProviderId")!),
                Guid.Parse(principal.FindFirstValue("serviceId")!),
                DateTime.UtcNow
            );
        }
        catch
        {
            return Error.Failure("Invalid or expired QR token.");
        }
    }
}
