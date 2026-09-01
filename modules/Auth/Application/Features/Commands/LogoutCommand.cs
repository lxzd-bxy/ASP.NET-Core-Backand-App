using ErrorOr;
using MediatR;
using LxzdBxy.Backend.Application.Features.Responses;

namespace LxzdBxy.Backend.Application.Features.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<ErrorOr<LogoutResponse>> { }