using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ItLxzdbxy.WebApi.Application.Interfaces;
using ItLxzdbxy.WebApi.Infrastructure.Configuration;
using ItLxzdbxy.WebApi.Application.DTOs;

namespace ItLxzdbxy.WebApi.Presentation.Controllers;

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
            AuthResult.Success success => HandleSuccess(success.AccessToken, success.User),
            AuthResult.Failure failure => HandleFailure(failure),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.")
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result switch
        {
            AuthResult.Success success => HandleSuccess(success.AccessToken, success.User),
            AuthResult.Failure failure => HandleFailure(failure),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.")
        };
    }

    private OkObjectResult HandleSuccess(string token, object? user)
    {
        var cookieOptions = _jwtOptions.Value.Cookie;
        cookieOptions.Expires ??= DateTime.UtcNow.AddMinutes(_jwtOptions.Value.AccessTokenLifetimeMinutes);

        Response.Cookies.Append("access_token", token, cookieOptions);

        return Ok(new
        {
            message = "Authentication successful",
            user = user
        });
    }

    private UnauthorizedObjectResult HandleFailure(AuthResult.Failure failure)
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