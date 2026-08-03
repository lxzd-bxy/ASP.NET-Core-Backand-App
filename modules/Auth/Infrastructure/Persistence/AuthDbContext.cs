using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LxzdBxy.WebApi.Domain.Entities;
using LxzdBxy.WebApi.Infrastructure.Identity;

namespace LxzdBxy.WebApi.Infrastructure.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
