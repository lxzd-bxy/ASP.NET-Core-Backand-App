using MediatR;
using ErrorOr;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<AuthResponse>>;