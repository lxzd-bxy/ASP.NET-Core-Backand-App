namespace ItLxzdbxy.WebApi.Application.Features.Auth.Responses;

public record LoginResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    int ExpiresIn
);