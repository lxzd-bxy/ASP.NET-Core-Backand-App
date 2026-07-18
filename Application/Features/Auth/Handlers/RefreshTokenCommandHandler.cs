using MediatR;
using ItLxzdbxy.WebApi.Application.Common.Interfaces;
using ItLxzdbxy.WebApi.Application.Common.Exceptions;
using ItLxzdbxy.WebApi.Application.Features.Auth.Commands;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(
    UserManager<IdentityUser> userManager,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository
    ) :
    IRequestHandler<RefreshTokenCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken, ct);

        if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.IsRevoked)
            return AuthException.InvalidRefreshToken;

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user is null)
            return AuthException.UserNotFound;

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshExpiry = _jwtService.GetRefreshTokenExpiryTime();

        storedToken.Token = newRefreshToken;
        storedToken.ExpiresAt = refreshExpiry;
        storedToken.CreatedAt = DateTime.UtcNow;
        storedToken.IsRevoked = false;

        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return new AuthResponse(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken,
            ExpiresAt: refreshExpiry);
    }
}