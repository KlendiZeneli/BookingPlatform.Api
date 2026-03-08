using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken ct);
    Task AddBooking(Booking booking);
    Task AddAsync(Property property, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
