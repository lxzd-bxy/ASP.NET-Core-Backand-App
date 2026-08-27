using MediatR;
using ErrorOr;
using LxzdBxy.Backend.Application.Features.Responses;

namespace LxzdBxy.Backend.Application.Features.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<RefreshResponse>> { }