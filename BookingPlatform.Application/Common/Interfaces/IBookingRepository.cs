using BookingPlatform.Domain.Entities;
using BookingPlatform.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IBookingRepository : ICrudRepository<Booking>
{
    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct);
}
