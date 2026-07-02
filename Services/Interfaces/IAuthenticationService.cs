using ItLxzdbxy.WebApi.DTOs.Auth;
using ItLxzdbxy.WebApi.Results;

namespace ItLxzdbxy.WebApi.Services.Interfaces;
public interface IAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
}