using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Domain.Entitties.Identity;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fayora.Infrastructure.Services.AuthServices;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    public int ExpiresIn => _jwtSettings.TokenExpirationInMinutes * 60;

    public string GenerateToken(string deviceId, User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
            new("device_id",                        deviceId),
            new("email_verified",                   user.IsEmailVerified.ToString().ToLower(), ClaimValueTypes.Boolean),
            new("phone_verified",                   user.IsPhoneVerified.ToString().ToLower(), ClaimValueTypes.Boolean),
        };

        if (!string.IsNullOrEmpty(user.PrimaryEmail?.Value))
        {
            claims.Add(new(JwtRegisteredClaimNames.Email, user.PrimaryEmail.Value));
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new("phone_number", user.PhoneNumber));
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
