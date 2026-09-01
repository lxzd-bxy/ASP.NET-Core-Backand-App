using System.Text;
using LxzdBxy.Backend.Presentation.Interfaces;
using LxzdBxy.Backend.Presentation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LxzdBxy.Backend.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddWebService(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["JwtOptions:SecretKey"] ?? string.Empty;
        var jwtIssuer = configuration["JwtOptions:Issuer"];
        var jwtAudience = configuration["JwtOptions:Audience"];

        if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            throw new InvalidOperationException("JWT configuration is missing required values.");

        services.AddControllers();
        services.AddOpenApi();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new StringTrimmerJsonConverter());
        });

        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ErrorOrHandler>();
        services.AddHttpContextAccessor();
        services.AddAuthorization();

        services.AddCors(opt =>
        {
            opt.AddPolicy(
                "AllowFrontend",
                policy =>
                {
                    policy
                        .WithOrigins("https://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }
}