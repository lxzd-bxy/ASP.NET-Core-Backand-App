using MediatR;
using ErrorOr;
using LxzdBxy.Backend.Application.Common.Exceptions;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Application.Features.Commands;
using LxzdBxy.Backend.Application.Features.Responses;

namespace LxzdBxy.Backend.Application.Features.Auth.Handlers;

public class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository) 
: IRequestHandler<LogoutCommand, ErrorOr<LogoutResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<ErrorOr<LogoutResponse>> Handle(LogoutCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        if (storedToken is null)
            return AuthException.InvalidRefreshToken;

        _refreshTokenRepository.Delete(storedToken);

        return new LogoutResponse();
    }
}