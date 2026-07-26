using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;

namespace LxzdBxy.WebApi.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthResponse>> { }