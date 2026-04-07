using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Entities.IdentityModule;
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

    public string GenerateToken(string deviceId, User user, Guid? touristId = null, Guid? tourGuideId = null, Guid? ownerId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),

            new(JwtRegisteredClaimNames.Name,       user.FirstName + " " + user.LastName),
            new(JwtRegisteredClaimNames.Picture,    user.ProfileImageUrl ?? string.Empty),
            new("device_id",                        deviceId),
            new("email_verified",                   user.IsEmailVerified.ToString(), ClaimValueTypes.Boolean),
            new("phone_verified",                   user.IsPhoneVerified.ToString(), ClaimValueTypes.Boolean),
        };

        if (!string.IsNullOrEmpty(user.PrimaryEmail?.Value))
        {
            claims.Add(new(ClaimTypes.Email, user.PrimaryEmail.Value));
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new(ClaimTypes.MobilePhone, user.PhoneNumber));
        }

        if (ownerId.HasValue)
        {
            claims.Add(new("owner_id", ownerId.Value.ToString()));
        }

        if (tourGuideId.HasValue)
        {
            claims.Add(new("tour_guide_id", tourGuideId.Value.ToString()));
        }

        if (touristId.HasValue)
        {
            claims.Add(new("tourist_id", touristId.Value.ToString()));
        }

        var roles = user.GetRoleNames();

        if (roles.Count != 0)
        {
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
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
