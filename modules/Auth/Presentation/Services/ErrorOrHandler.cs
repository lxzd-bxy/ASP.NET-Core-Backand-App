using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ErrorOr;

namespace LxzdBxy.Backend.Presentation.Services;

public class ErrorOrHandler(ProblemDetailsFactory problemDetailsFactory)
{
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    public IActionResult HandleErrorOr<T>(
        ErrorOr<T> result,
        // Func<T, IActionResult> onSuccess,
        HttpContext httpContext)
    {
        // if (!result.IsError)
        //     return onSuccess(result.Value);

        var firstError = result.Errors[0];
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: statusCode,
            title: "Error",
            detail: "One or more errors occurred."
        );

        problemDetails.Extensions["errors"] = result.Errors
            .Select(e => new { e.Code, e.Description })
            .ToList();

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}