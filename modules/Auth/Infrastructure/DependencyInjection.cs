using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LxzdBxy.Backend.Infrastructure.Persistence;
using LxzdBxy.Backend.Infrastructure.Configurations;
using LxzdBxy.Backend.Infrastructure.Authentication;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Infrastructure.Persistence.Repositories;
using LxzdBxy.Backend.Infrastructure.Identity;

namespace LxzdBxy.Backend.Infrastructure;

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