using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MediatR;
using ErrorOr;
using ItLxzdbxy.WebApi.Application.Common.Requests;
using ItLxzdbxy.WebApi.Application.Features.Auth.Commands;
using ItLxzdbxy.WebApi.Infrastructure.Authentication;

namespace ItLxzdbxy.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(
    IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        return HandleErrorOr(result, success => Ok(new { Token = success.AccessToken }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);
        return HandleErrorOr(result, success => Ok(new { Token = success.AccessToken }));
    }

    private IActionResult HandleErrorOr<T>(ErrorOr<T> result, Func<T, IActionResult> onSuccess)
    {
        return result.Match(
            success => onSuccess(success),
            errors => Problem(errors.First().Description, statusCode: 400)
        );
    }
}