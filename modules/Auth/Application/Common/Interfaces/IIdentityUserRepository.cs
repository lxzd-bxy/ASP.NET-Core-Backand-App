using ErrorOr;
using LxzdBxy.Backend.Application.Common.Models;

namespace LxzdBxy.Backend.Application.Common.Interfaces;

public interface IIdentityUserRepository
{
    Task<UserClaimsDto> FindByIdAsync(string id);
    Task<UserClaimsDto?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(UserClaimsDto user, string password);
    Task<ErrorOr<UserClaimsDto>> CreateAsync(string email, string password);
}