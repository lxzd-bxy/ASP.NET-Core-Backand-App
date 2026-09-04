using LxzdBxy.Backend.Infrastructure.Configurations;
using LxzdBxy.Backend.Presentation.Interfaces;

namespace LxzdBxy.Backend.Presentation.Services;

public class CookieService(JwtSettings jwtSettings) : ICookieService
{
    private readonly JwtSettings _jwtSettings = jwtSettings;
    public void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
    {
        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays),
            Path = "/api/refresh"
        });
    }

    public void ClearRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("refreshToken", new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Path = "/"
    });
    }

    public string? GetRefreshTokenFromRequest(HttpRequest request)
    {
        return request.Cookies["refreshToken"];
    }
}