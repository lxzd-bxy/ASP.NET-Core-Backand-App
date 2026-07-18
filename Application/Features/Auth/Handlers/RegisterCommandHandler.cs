using Microsoft.AspNetCore.Identity;
using MediatR;
using ErrorOr;
using ItLxzdbxy.WebApi.Application.Common.Interfaces;
using ItLxzdbxy.WebApi.Application.Features.Auth.Commands;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(UserManager<IdentityUser> userManager, IJwtService jwtService) : IRequestHandler<RegisterCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new IdentityUser(request.Email)
        {
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
            .Select(e => Error.Validation(
                code: e.Code,
                description: e.Description
            )).ToList();

            return errors;
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var authSuccess = new AuthResponse(accessToken, refreshToken, DateTime.Now);

        return authSuccess;
    }
}