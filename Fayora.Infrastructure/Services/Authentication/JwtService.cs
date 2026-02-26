using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fayora.Infrastructure.Services.Authentication;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(string deviceId, User user,IEnumerable<string>? roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.GivenName,  user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
            new("status",                           user.Status.ToString())
        };


        if (!string.IsNullOrEmpty(user.PrimaryEmail?.Value) && user.IsEmailVerified)
            claims.Add(new(JwtRegisteredClaimNames.EmailVerified, user.PrimaryEmail.Value));
        else if (!string.IsNullOrEmpty(user.PrimaryEmail?.Value))
            claims.Add(new(JwtRegisteredClaimNames.Email, user.PrimaryEmail.Value));

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            claims.Add(new("phone", user.PhoneNumber));

        if(roles is not null)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        

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
