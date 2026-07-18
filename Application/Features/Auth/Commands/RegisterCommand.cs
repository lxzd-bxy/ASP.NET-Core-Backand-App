using ErrorOr;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;
using MediatR;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<ErrorOr<AuthResponse>>;