using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ItLxzdbxy.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using ItLxzdbxy.WebApi.Services.Interfaces;
using ItLxzdbxy.WebApi.Services.Auth;
using ItLxzdbxy.WebApi.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerUI();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddAuthorization();
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        bool canConnect = await dbContext.Database.CanConnectAsync();
        if (canConnect)
        {
            logger.LogInformation("DB works!");
        }
        else
        {
            logger.LogWarning("DB down!");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error with connection to DB");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();