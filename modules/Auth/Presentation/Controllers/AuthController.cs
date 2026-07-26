using Microsoft.AspNetCore.Mvc;
using MediatR;
using LxzdBxy.WebApi.Application.Common.Requests;
using LxzdBxy.WebApi.Application.Features.Auth.Commands;
using ErrorOr;

namespace LxzdBxy.WebApi.Presentation.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        return HandleErrorOr(result, success => Ok(new
        {
            success.AccessToken
        }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);
        return HandleErrorOr(result, success => Ok(new
        {
            success.AccessToken
        }));
    }

    private IActionResult HandleErrorOr<T>(ErrorOr<T> result, Func<T, IActionResult> onSuccess)
    {
        if (!result.IsError)
            return onSuccess(result.Value);

        var fiirstError = result.Errors[0];
        var statusCode = fiirstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            detail: "One or more errors occurred.",
            statusCode: statusCode,
            title: "Error",
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = result.Errors.Select(e => new { e.Code, e.Description })
            }
        );
    }
}