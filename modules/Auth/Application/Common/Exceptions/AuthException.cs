using ErrorOr;

namespace LxzdBxy.WebApi.Application.Common.Exceptions;

public static class AuthException
{
    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        code: "Auth.InvalidRefreshToken",
        description: "Invalid or expired refresh token.");

    public static readonly Error UserNotFound = Error.NotFound(
        code: "Auth.UserNotFound",
        description: "User associated with refresh token not found.");
    public static readonly Error IncorrectPassword = Error.Unauthorized(
        code: "Auth.IncorrectPassword",
        description: "Invalid password.");
}