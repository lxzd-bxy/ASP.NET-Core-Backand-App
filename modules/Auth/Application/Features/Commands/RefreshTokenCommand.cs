using MediatR;
using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Responses;

namespace LxzdBxy.WebApi.Application.Features.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<RefreshResponse>> { }