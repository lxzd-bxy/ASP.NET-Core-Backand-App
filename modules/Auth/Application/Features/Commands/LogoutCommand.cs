using ErrorOr;
using MediatR;
using LxzdBxy.WebApi.Application.Features.Responses;

namespace LxzdBxy.WebApi.Application.Features.Commands;

public record LogoutCommand : IRequest<ErrorOr<LogoutResponse>> { }