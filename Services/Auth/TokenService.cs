using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ItLxzdbxy.WebApi.Options;
using ItLxzdbxy.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ItLxzdbxy.WebApi.Services.Auth;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            },
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}