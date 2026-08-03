using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LxzdBxy.WebApi.Infrastructure.Persistence;
using LxzdBxy.WebApi.Infrastructure.Configurations;
using LxzdBxy.WebApi.Infrastructure.Authentication;
using LxzdBxy.WebApi.Application.Common.Interfaces;
using LxzdBxy.WebApi.Infrastructure.Persistence.Repositories;
using LxzdBxy.WebApi.Infrastructure.Identity;

namespace LxzdBxy.WebApi.Infrastructure;

public static class DepedencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtOptions").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing or invalid.");
        services.AddSingleton(jwtSettings);
        services.AddDbContext<AuthDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.Configure<JwtSettings>(
            configuration.GetSection("JwtOptions"));
        return services;
    }
}