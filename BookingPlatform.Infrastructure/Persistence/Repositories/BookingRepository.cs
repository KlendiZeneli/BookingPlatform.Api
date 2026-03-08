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

    public async Task AddAsync(Booking booking, CancellationToken ct)
    {
        await _context.Bookings.AddAsync(booking, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
