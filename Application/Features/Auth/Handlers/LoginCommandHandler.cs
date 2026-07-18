using Microsoft.AspNetCore.Identity;
using MediatR;
using ErrorOr;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;
using ItLxzdbxy.WebApi.Application.Features.Auth.Commands;
using ItLxzdbxy.WebApi.Application.Common.Interfaces;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Handlers;

public class LoginCommandHandler(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IJwtService jwtService
) : IRequestHandler<LoginCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ErrorOr<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Error.Unauthorized("Invalid email or password.");

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: true
        );

        if (!signInResult.Succeeded)
        {
            return Error.Unauthorized("Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var authSuccess = new AuthResponse(accessToken, refreshToken, DateTime.Now);

        return authSuccess;
    }
}