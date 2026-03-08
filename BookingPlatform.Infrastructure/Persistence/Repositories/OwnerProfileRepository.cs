using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public async Task AddAsync(OwnerProfile profile, CancellationToken ct)
    {
        await _context.OwnerProfiles.AddAsync(profile, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
