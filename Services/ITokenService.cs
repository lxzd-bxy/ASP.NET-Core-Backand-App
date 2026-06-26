using Microsoft.AspNetCore.Identity;

namespace ItLxzdbxy.WebApi.Services;
public interface ITokenService
{
    string GenerateToken(IdentityUser user);
}