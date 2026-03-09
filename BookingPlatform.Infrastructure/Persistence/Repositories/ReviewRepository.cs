using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Review?> GetByBookingAndGuestAsync(Guid bookingId, Guid guestId, CancellationToken ct)
    {
        return _context.Reviews.FirstOrDefaultAsync(r => r.BookingId == bookingId && r.GuestId == guestId, ct);
    }

    public Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct)
        => _context.Reviews
            .Where(r => r.PropertyId == propertyId)
            .Include(r => r.Guest)
            .ToListAsync(ct).ContinueWith(t => (IEnumerable<Review>)t.Result, ct);

    public Task<Review?> GetByIdAsync(Guid id, CancellationToken ct)
        => _context.Reviews.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<IEnumerable<Review>> GetAllAsync(CancellationToken ct)
        => _context.Reviews.ToListAsync(ct).ContinueWith(t => (IEnumerable<Review>)t.Result, ct);

    public Task UpdateAsync(Review entity, CancellationToken ct)
    {
        _context.Reviews.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var r = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r != null) _context.Reviews.Remove(r);
    }

    public async Task AddAsync(Review review, CancellationToken ct)
    {
        await _context.Reviews.AddAsync(review, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
