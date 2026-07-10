using ItLxzdbxy.WebApi.Application.DTOs;
using ItLxzdbxy.WebApi.Application.Interfaces;
using ItLxzdbxy.WebApi.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ItLxzdbxy.WebApi.Application.Services;

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
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("User {UserId} logged in successfully.", user.Id);
        }

        var userDto = new { id = user.Id, email = user.Email };
        return new AuthResult.Success(token, userDto);
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            return new AuthResult.Failure("Email is already registered.", "EMAIL_EXISTS");
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var err = string.Join("; ", createResult.Errors.Select(e => e.Description));
            logger.LogWarning("User creation failed for {Email}: {Errors}", request.Email, err);
            return new AuthResult.Failure("Registration failed.", "REGISTRATION_FAILED");
        }

        var token = tokenService.GenerateToken(user);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("User {UserId} registered successfully.", user.Id);
        }

        var userDto = new { id = user.Id, email = user.Email };
        return new AuthResult.Success(token, userDto);
    }
}