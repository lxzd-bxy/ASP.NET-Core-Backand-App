using LxzdBxy.Backend.Domain.Entities;

namespace LxzdBxy.Backend.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
    void Update(RefreshToken refreshToken);
    void Add(RefreshToken refreshToken);
    void Delete(RefreshToken refreshToken);
    Task SaveChangesAsync(CancellationToken ct);
}