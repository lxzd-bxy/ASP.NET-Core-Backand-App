using MediatR;
using ErrorOr;
using LxzdBxy.Backend.Application.Features.Responses;

namespace LxzdBxy.Backend.Application.Features.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<LoginResponse>>;