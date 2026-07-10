using ItLxzdbxy.WebApi.Application.DTOs;

namespace ItLxzdbxy.WebApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
}