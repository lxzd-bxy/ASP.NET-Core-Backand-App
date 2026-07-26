using LxzdBxy.WebApi.Application.Common.Models;

namespace LxzdBxy.WebApi.Application.Common.Interfaces;

public interface IIdentityUserRepository
{
    Task<UserClaimsDto?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(UserClaimsDto user, string password);
}