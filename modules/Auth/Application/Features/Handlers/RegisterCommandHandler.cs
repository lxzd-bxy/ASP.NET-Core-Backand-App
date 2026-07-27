using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Auth.Commands;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;
using LxzdBxy.WebApi.Application.Common.Interfaces;

namespace LxzdBxy.WebApi.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(IIdentityUserRepository userRepository, IJwtService jwtService)
: IRequestHandler<RegisterCommand, ErrorOr<AuthResponse>>
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existingUser = await _userRepository.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Error.Conflict(description: "Пользователь с таким email уже зарегистрирован");

        var createResult = await _userRepository.CreateAsync(request.Email, request.Password);
        if (createResult.IsError)
            return createResult.Errors;
        var accessToken = _jwtService.GenerateAccessToken(createResult.Value);
        return new AuthResponse(accessToken);
    }
}