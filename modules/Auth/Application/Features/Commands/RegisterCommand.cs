using ErrorOr;
using LxzdBxy.WebApi.Application.Features.Responses;
using MediatR;

namespace LxzdBxy.WebApi.Application.Features.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<ErrorOr<RegisterResponse>>;