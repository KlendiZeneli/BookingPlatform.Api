using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct)
    {
        return _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
    }

    public Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct)
    {
        return _context.Bookings.ToListAsync(ct).ContinueWith(t => (IEnumerable<Booking>)t.Result, ct);
    }

    public Task UpdateAsync(Booking entity, CancellationToken ct)
    {
        _context.Bookings.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var b = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b != null) _context.Bookings.Remove(b);
    }

    public async Task AddAsync(Booking booking, CancellationToken ct)
    {
        await _context.Bookings.AddAsync(booking, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
