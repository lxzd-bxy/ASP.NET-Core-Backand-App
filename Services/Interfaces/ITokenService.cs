using Microsoft.AspNetCore.Identity;

namespace ItLxzdbxy.WebApi.Services.Interfaces;
public interface ITokenService
{
    string GenerateToken(IdentityUser user);
}