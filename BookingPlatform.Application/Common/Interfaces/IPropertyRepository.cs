using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IPropertyRepository : ICrudRepository<Property>
{
    Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken ct);
    Task AddBooking(Booking booking);
}
