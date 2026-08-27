using LxzdBxy.Backend.Application.Common.Models;

namespace LxzdBxy.Backend.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(UserClaimsDto user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiryTime();
}