namespace ItLxzdbxy.WebApi.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? RefreshToken { get; }
}
