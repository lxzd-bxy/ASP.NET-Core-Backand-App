using Microsoft.AspNetCore.Identity;
using ErrorOr;
using LxzdBxy.Backend.Infrastructure.Identity;
using LxzdBxy.Backend.Application.Common.Models;
using LxzdBxy.Backend.Application.Common.Interfaces;

namespace LxzdBxy.Backend.Infrastructure.Persistence.Repositories;

public class IdentityUserRepository(UserManager<AppUser> userManager) : IIdentityUserRepository
{
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task<UserClaimsDto> FindByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(id));

        var user = await _userManager.FindByIdAsync(id) ?? throw new InvalidOperationException("User not found.");
        return new UserClaimsDto(Id: user.Id, Email: user.Email ?? string.Empty);
    }

    public async Task<UserClaimsDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null
            ? null
            : new UserClaimsDto(Id: user.Id, Email: user.Email ?? string.Empty);
    }

    public async Task<bool> CheckPasswordAsync(UserClaimsDto user, string password)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        return identityUser is not null && await _userManager.CheckPasswordAsync(identityUser, password);
    }

    public async Task<ErrorOr<UserClaimsDto>> CreateAsync(string email, string password)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
            return errors;
        }

        return new UserClaimsDto(Id: user.Id, Email: user.Email);
    }
}