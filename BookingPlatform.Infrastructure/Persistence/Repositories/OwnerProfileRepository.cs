using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class OwnerProfileRepository : IOwnerProfileRepository
{
    private readonly AppDbContext _context;

    public OwnerProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<OwnerProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return _context.OwnerProfiles.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public Task<OwnerProfile?> GetByIdAsync(Guid id, CancellationToken ct)
        => _context.OwnerProfiles.FirstOrDefaultAsync(p => p.UserId == id, ct);

    public Task<IEnumerable<OwnerProfile>> GetAllAsync(CancellationToken ct)
        => _context.OwnerProfiles.ToListAsync(ct).ContinueWith(t => (IEnumerable<OwnerProfile>)t.Result, ct);

    public Task UpdateAsync(OwnerProfile entity, CancellationToken ct)
    {
        _context.OwnerProfiles.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var p = await _context.OwnerProfiles.FirstOrDefaultAsync(x => x.UserId == id, ct);
        if (p != null) _context.OwnerProfiles.Remove(p);
    }

    public async Task AddAsync(OwnerProfile profile, CancellationToken ct)
    {
        await _context.OwnerProfiles.AddAsync(profile, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
