using ItLxzdbxy.WebApi.Application.Common.Interfaces;
using ItLxzdbxy.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItLxzdbxy.WebApi.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
	private readonly AppDbContext _context = context;

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