namespace LxzdBxy.WebApi.Presentation.Interfaces;

public interface ICookieService
{
    void SetRefreshTokenCookie(HttpResponse response, string refreshToken);
    void ClearRefreshTokenCookie(HttpResponse response);
    string? GetRefreshTokenFromRequest(HttpRequest request);
}