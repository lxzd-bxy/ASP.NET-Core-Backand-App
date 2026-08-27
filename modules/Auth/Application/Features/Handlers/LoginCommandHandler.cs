using MediatR;
using ErrorOr;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Application.Common.Exceptions;
using LxzdBxy.Backend.Application.Features.Commands;
using LxzdBxy.Backend.Application.Features.Responses;
using LxzdBxy.Backend.Domain.Entities;

namespace LxzdBxy.Backend.Application.Features.Auth.Handlers;

public class LoginCommandHandler(
    IIdentityUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService)
: IRequestHandler<LoginCommand, ErrorOr<LoginResponse>>
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return AuthException.UserNotFound;

        var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid) return AuthException.IncorrectPassword;

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _refreshTokenRepository.Add(refreshTokenEntity);

        await _refreshTokenRepository.SaveChangesAsync(ct);

        return new LoginResponse(accessToken, refreshToken);
    }
}