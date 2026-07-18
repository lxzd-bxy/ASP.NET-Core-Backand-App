using ErrorOr;
using MediatR;
using ItLxzdbxy.WebApi.Application.Features.Auth.Responses;

namespace ItLxzdbxy.WebApi.Application.Features.Auth.Commands;

public record LogoutCommand : IRequest<ErrorOr<AuthResponse>> { }