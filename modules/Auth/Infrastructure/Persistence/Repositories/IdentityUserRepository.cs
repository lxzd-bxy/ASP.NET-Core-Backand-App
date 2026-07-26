using Microsoft.AspNetCore.Identity;
using LxzdBxy.WebApi.Application.Common.Models;
using LxzdBxy.WebApi.Application.Common.Interfaces;

namespace LxzdBxy.WebApi.Infrastructure.Persistence.Repositories;

public class IdentityUserRepository(UserManager<IdentityUser> userManager) : IIdentityUserRepository
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<UserClaimsDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null
            ? null
            : new UserClaimsDto(user.Id, user.Email ?? string.Empty, user.PasswordHash ?? string.Empty);
    }

    public async Task<bool> CheckPasswordAsync(UserClaimsDto user, string password)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return identityUser is not null && await _userManager.CheckPasswordAsync(identityUser, password);
    }
}