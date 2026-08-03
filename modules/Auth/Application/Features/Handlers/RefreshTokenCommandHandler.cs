using MediatR;
using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Application.Common.Exceptions;
using LxzdBxy.WebApi.Application.Features.Commands;
using LxzdBxy.WebApi.Application.Features.Responses;
using ErrorOr;

namespace LxzdBxy.WebApi.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(
    IIdentityUserRepository userRepository,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository
    ) :
    IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshResponse>>
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<RefreshResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken, ct);

        if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.IsRevoked)
            return AuthException.InvalidRefreshToken;

        var user = await _userRepository.FindByIdAsync(storedToken.UserId);
        if (user is null)
            return AuthException.UserNotFound;

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshExpiry = _jwtService.GetRefreshTokenExpiryTime();

        storedToken.Token = newRefreshToken;
        storedToken.CreatedAt = DateTime.UtcNow;
        storedToken.IsRevoked = false;

        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return new RefreshResponse(AccessToken: newAccessToken);
    }
}