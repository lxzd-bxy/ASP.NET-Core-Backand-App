using Microsoft.AspNetCore.Mvc;
using MediatR;
using LxzdBxy.Backend.Application.Common.Requests;
using LxzdBxy.Backend.Application.Features.Commands;
using LxzdBxy.Backend.Presentation.Services;
using LxzdBxy.Backend.Presentation.Interfaces;
using ErrorOr;

namespace LxzdBxy.Backend.Presentation.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(
    IMediator mediator,
    ICookieService cookieService,
    ErrorOrHandler errorOrHandler
    ) : ControllerBase
{
    private readonly ErrorOrHandler _errorOrHandler = errorOrHandler;
    private readonly IMediator _mediator = mediator;
    private readonly ICookieService _cookieService = cookieService;


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        if (result.IsError)
            return _errorOrHandler.HandleErrorOr(result, HttpContext);

        _cookieService.SetRefreshTokenCookie(Response, result.Value.RefreshToken);

        return Ok(new { result.Value.AccessToken });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        if (result.IsError)
            return _errorOrHandler.HandleErrorOr(result, HttpContext);

        _cookieService.SetRefreshTokenCookie(Response, result.Value.RefreshToken);

        return Ok(new { result.Value.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = _cookieService.GetRefreshTokenFromRequest(Request);

        if (!string.IsNullOrEmpty(refreshToken))
        {
            // await _authService.RevokeRefreshTokenAsync(refreshToken, ct);
            _cookieService.ClearRefreshTokenCookie(Response);
        }
        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = _cookieService.GetRefreshTokenFromRequest(Request);
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var command = new RefreshTokenCommand(refreshToken);
        var result = await _mediator.Send(command);
        if (result.IsError)
            return _errorOrHandler.HandleErrorOr(result, HttpContext);

        return Ok(new { result.Value.AccessToken });
    }
}