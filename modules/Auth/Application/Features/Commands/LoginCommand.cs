using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Responses;

namespace LxzdBxy.WebApi.Application.Features.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<LoginResponse>>;