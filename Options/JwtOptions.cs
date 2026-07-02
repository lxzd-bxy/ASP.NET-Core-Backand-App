namespace ItLxzdbxy.WebApi.Options;

public class JwtOptions
{
    public int AccessTokenLifetimeMinutes { get; set; } = 15;
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public CookieOptions Cookie { get; set; } = new();
}