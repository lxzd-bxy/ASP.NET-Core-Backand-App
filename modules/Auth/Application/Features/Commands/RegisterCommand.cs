using ErrorOr;
using LxzdBxy.Backend.Application.Features.Responses;
using MediatR;

namespace LxzdBxy.Backend.Application.Features.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<ErrorOr<RegisterResponse>>;