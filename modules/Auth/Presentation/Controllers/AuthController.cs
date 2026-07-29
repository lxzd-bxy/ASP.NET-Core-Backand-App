using Microsoft.AspNetCore.Mvc;
using MediatR;
using LxzdBxy.WebApi.Application.Common.Requests;
using LxzdBxy.WebApi.Application.Features.Auth.Commands;
using LxzdBxy.WebApi.Presentation.Services;
using LxzdBxy.WebApi.Presentation.Interfaces;

namespace LxzdBxy.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(IMediator mediator, ICookieService cookieService, ErrorOrHandler errorOrHandler) : ControllerBase
{
    private readonly ErrorOrHandler _errorOrHandler = errorOrHandler;
    private readonly IMediator _mediator = mediator;
    private readonly ICookieService _cookieService = cookieService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        _cookieService.SetRefreshTokenCookie(Response, result.Value.RefreshToken);

        return _errorOrHandler.HandleErrorOr(result, success => Ok(new { success.AccessToken }), HttpContext);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        _cookieService.SetRefreshTokenCookie(Response, result.Value.RefreshToken);

        return _errorOrHandler.HandleErrorOr(result, success => Ok(new { success.AccessToken }), HttpContext);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = _cookieService.GetRefreshTokenFromRequest(Request);
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var command = new RefreshTokenCommand(refreshToken);
        var result = await _mediator.Send(command);
        if (result.IsError)
            return Unauthorized(result.Errors);

        return Ok(new { AccessToken = result.Value });
    }
}