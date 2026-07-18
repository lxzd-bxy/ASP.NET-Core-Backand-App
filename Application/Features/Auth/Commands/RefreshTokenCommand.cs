using MediatR;
using ErrorOr;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthResponse>> { }