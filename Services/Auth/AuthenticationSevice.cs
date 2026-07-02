using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ItLxzdbxy.WebApi.DTOs.Auth;
using ItLxzdbxy.WebApi.Options;
using ItLxzdbxy.WebApi.Results;
using ItLxzdbxy.WebApi.Services.Interfaces;

namespace ItLxzdbxy.WebApi.Services.Auth;

public class AuthenticationService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthenticationService> logger) : IAuthenticationService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
            return new AuthResult.Failure("Invalid email or password.", "INVALID_CREDENTIALS");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            logger.LogWarning("Failed login attempt for user: {UserId}", user.Id);
            return new AuthResult.Failure("Invalid email or password.", "INVALID_CREDENTIALS");
        }

        var token = tokenService.GenerateToken(user);
        logger.LogInformation("User {UserId} logged in successfully.", user.Id);
        return new AuthResult.Success(token);
    }
}