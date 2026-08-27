using Microsoft.EntityFrameworkCore;
using LxzdBxy.Backend.Application.Common.Interfaces;
using LxzdBxy.Backend.Domain.Entities;

namespace LxzdBxy.Backend.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AuthDbContext context) : IRefreshTokenRepository
{
	private readonly AuthDbContext _context = context;

	public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
	{
		return await _context.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == token, ct);
	}

	public void Update(RefreshToken refreshToken)
	{
		_context.Entry(refreshToken).State = EntityState.Modified;
	}

	public void Add(RefreshToken refreshToken)
	{
		_context.RefreshTokens.Add(refreshToken);
	}

	public async Task SaveChangesAsync(CancellationToken ct)
	{
		await _context.SaveChangesAsync(ct);
	}
}