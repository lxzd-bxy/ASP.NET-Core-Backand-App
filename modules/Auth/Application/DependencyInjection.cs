using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using LxzdBxy.WebApi.Application.Common.Behaviors;

namespace LxzdBxy.WebApi.Application;

public static class DepedencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}