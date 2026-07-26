using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;

namespace LxzdBxy.WebApi.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<AuthResponse>>;