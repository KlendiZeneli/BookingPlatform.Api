using BookingPlatform.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IReviewRepository : ICrudRepository<Review>
{
    Task<Review?> GetByBookingAndGuestAsync(Guid bookingId, Guid guestId, CancellationToken ct);
    Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct);
}
