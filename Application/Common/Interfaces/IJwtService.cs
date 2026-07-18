using Microsoft.AspNetCore.Identity;

namespace ItLxzdbxy.WebApi.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(IdentityUser user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiryTime();
}