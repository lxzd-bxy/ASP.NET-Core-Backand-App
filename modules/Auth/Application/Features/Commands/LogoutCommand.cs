using ErrorOr;
using MediatR;
using LxzdBxy.WebApi.Application.Features.Auth.Responses;

namespace LxzdBxy.WebApi.Application.Features.Auth.Commands;

public record LogoutCommand : IRequest<ErrorOr<AuthResponse>> { }