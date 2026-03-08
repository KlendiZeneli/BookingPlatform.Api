using BookingPlatform.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct);
    Task AddAsync(Booking booking, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
