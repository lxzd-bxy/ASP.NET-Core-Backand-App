using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Infrastructure.Configurations;
using LxzdBxy.WebApi.Infrastructure.Persistence.Repositories;
using LxzdBxy.WebApi.Presentation.Interfaces;

namespace LxzdBxy.WebApi.Presentation.Services;

public class CookieService(JwtSettings jwtSettings, RefreshTokenRepository refreshTokenRepository) : ICookieService
{
    private readonly JwtSettings _jwtSettings = jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    public void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
    {
        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays),
            Path = "/auth/refresh"
        });
    }

    public void ClearRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("refreshToken");
    }

    public string? GetRefreshTokenFromRequest(HttpRequest request)
    {
        return request.Cookies["refreshToken"];
    }
}