using ItLxzdbxy.WebApi.DTOs.Auth;
using ItLxzdbxy.WebApi.Options;
using ItLxzdbxy.WebApi.Results;
using ItLxzdbxy.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ItLxzdbxy.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(
    IAuthenticationService authService,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthenticationService _authService = authService;
    private readonly IOptions<JwtOptions> _jwtOptions = jwtOptions;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.AuthenticateAsync(request, cancellationToken);

        return result switch
        {
            AuthResult.Success success => HandleSuccess(success.AccessToken),
            AuthResult.Failure failure => HandleFailure(failure),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.")
        };
    }

    private IActionResult HandleSuccess(string token)
    {
        var cookieOptions = _jwtOptions.Value.Cookie;
        cookieOptions.Expires ??= DateTime.UtcNow.AddMinutes(_jwtOptions.Value.AccessTokenLifetimeMinutes);

        Response.Cookies.Append("access_token", token, cookieOptions);

        return Ok(new
        {
            message = "Login successful",
        });
    }

    private IActionResult HandleFailure(AuthResult.Failure failure)
    {
        var problem = new ProblemDetails
        {
            Title = "Authentication failed",
            Detail = failure.Error,
            Status = StatusCodes.Status401Unauthorized,
            Extensions = { ["errorCode"] = failure.ErrorCode ?? "UNAUTHORIZED" }
        };

        return Unauthorized(problem);
    }
}