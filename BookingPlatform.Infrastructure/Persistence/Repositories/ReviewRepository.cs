using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public async Task AddAsync(Review review, CancellationToken ct)
    {
        await _context.Reviews.AddAsync(review, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
