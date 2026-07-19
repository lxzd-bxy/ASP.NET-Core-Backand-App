namespace ItLxzdbxy.WebApi.Infrastructure.Authentication;

public class JwtOptions
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public CookieOptions Cookie { get; set; } = new();
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}