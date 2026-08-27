using MediatR;
using ErrorOr;
using LxzdBxy.Backend.Application.Features.Commands;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Domain.Entities;
using LxzdBxy.Backend.Application.Features.Responses;

namespace LxzdBxy.Backend.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(
    IIdentityUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService)
: IRequestHandler<RegisterCommand, ErrorOr<RegisterResponse>>
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<RegisterResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existingUser = await _userRepository.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Error.Conflict(description: "A user with this email already exists");

        var createResult = await _userRepository.CreateAsync(request.Email, request.Password);
        if (createResult.IsError)
            return createResult.Errors;

        var accessToken = _jwtService.GenerateAccessToken(createResult.Value);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = createResult.Value.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _refreshTokenRepository.Add(refreshTokenEntity);

        await _refreshTokenRepository.SaveChangesAsync(ct);

        return new RegisterResponse(accessToken, refreshToken);
    }
}