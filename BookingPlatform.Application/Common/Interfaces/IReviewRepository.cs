using BookingPlatform.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByBookingAndGuestAsync(Guid bookingId, Guid guestId, CancellationToken ct);
    Task AddAsync(Review review, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
