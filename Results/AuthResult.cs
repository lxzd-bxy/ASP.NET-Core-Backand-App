namespace ItLxzdbxy.WebApi.Results;

public abstract record AuthResult
{
    public record Success(string AccessToken) : AuthResult;
    public record Failure(string Error, string? ErrorCode = null) : AuthResult;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;
}