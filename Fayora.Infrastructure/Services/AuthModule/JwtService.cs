using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fayora.Infrastructure.Services.AuthModule;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    public int ExpiresIn => _jwtSettings.TokenExpirationInMinutes * 60;

    public string GenerateToken(string deviceId, User user, bool isAccountVerified = true)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name,       $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Picture,    user.ProfileImageUrl?.Value ?? string.Empty),
            new(JwtRegisteredClaimNames.Email,      user.PrimaryEmail?.Value ?? string.Empty),
            new("phone",                           user.PhoneNumber?.Value ?? string.Empty),
            new("device_id",                        deviceId),
            new("email_verified",                   user.IsEmailVerified.ToString().ToLower(), ClaimValueTypes.Boolean),
            new("phone_verified",                   user.IsPhoneVerified.ToString().ToLower(), ClaimValueTypes.Boolean)
        };

        if (user.Roles.HasValue)
        {
            foreach (var role in Enum.GetValues<Role>())
            {
                if (role != 0 && user.Roles.Value.HasFlag(role))
                {
                    claims.Add(new System.Security.Claims.Claim("roles", role.ToString()));
                }
            }
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
