using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LxzdBxy.Backend.Domain.Entities;
using LxzdBxy.Backend.Infrastructure.Identity;

namespace LxzdBxy.Backend.Infrastructure.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
