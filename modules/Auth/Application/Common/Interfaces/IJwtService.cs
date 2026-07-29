using LxzdBxy.WebApi.Application.Common.Models;

namespace LxzdBxy.WebApi.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(UserClaimsDto user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiryTime();
}