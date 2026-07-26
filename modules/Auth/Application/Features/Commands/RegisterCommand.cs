using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;
using MediatR;

namespace LxzdBxy.WebApi.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<ErrorOr<AuthResponse>>;