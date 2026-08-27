using MediatR;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Application.Common.Exceptions;
using LxzdBxy.Backend.Application.Features.Commands;
using LxzdBxy.Backend.Application.Features.Responses;
using ErrorOr;

namespace LxzdBxy.Backend.Application.Features.Auth.Handlers;

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
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        Console.WriteLine($"Stored Token: {storedToken?.Token}, Expires At: {storedToken?.ExpiresAt}");

        if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow)
            return AuthException.InvalidRefreshToken;

        var user = await _userRepository.FindByIdAsync(storedToken.UserId);
        if (user is null)
            return AuthException.UserNotFound;

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        return new RefreshResponse(AccessToken: newAccessToken);
    }
}