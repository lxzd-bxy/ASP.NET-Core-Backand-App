using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Application.Common.Exceptions;
using LxzdBxy.WebApi.Application.Features.Auth.Commands;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;

namespace LxzdBxy.WebApi.Application.Features.Auth.Handlers;

public class LoginCommandHandler(IIdentityUserRepository userRepository, IJwtService jwtService) : IRequestHandler<LoginCommand, ErrorOr<AuthResponse>>
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ErrorOr<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return AuthException.UserNotFound;
        var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid) return AuthException.IncorrectPassword;
        var accessToken = _jwtService.GenerateAccessToken(user);

        return new AuthResponse(accessToken);
    }
}