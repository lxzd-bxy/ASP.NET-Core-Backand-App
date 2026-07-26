using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LxzdBxy.WebApi.Domain.Entities;

namespace LxzdBxy.WebApi.Infrastructure.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
