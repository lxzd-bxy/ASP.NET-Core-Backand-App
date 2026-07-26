using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Application.Common.Models;
using LxzdBxy.WebApi.Infrastructure.Configurations;

namespace LxzdBxy.WebApi.Infrastructure.Authentication;

public class JwtService(IOptions<JwtSettings> jwtOptions) : IJwtService
{
    private readonly JwtSettings _jwtOptions = jwtOptions.Value;

    public string GenerateAccessToken(UserClaimsDto user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ],
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}