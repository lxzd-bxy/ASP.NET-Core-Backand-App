using LxzdBxy.WebApi.Domain.Entities;

namespace LxzdBxy.WebApi.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
    void Update(RefreshToken refreshToken);
    void Add(RefreshToken refreshToken);
    Task SaveChangesAsync(CancellationToken ct);
}