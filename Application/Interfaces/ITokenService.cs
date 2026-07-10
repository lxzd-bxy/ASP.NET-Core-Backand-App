using Microsoft.AspNetCore.Identity;

namespace ItLxzdbxy.WebApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(IdentityUser user);
}