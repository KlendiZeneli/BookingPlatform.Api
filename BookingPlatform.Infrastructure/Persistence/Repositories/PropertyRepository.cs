using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Property property, CancellationToken ct)
    {
        await _context.Properties.AddAsync(property, ct);
    }

    public async Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken ct)
    {
        return await _context.Properties
            .Include(p => p.PropertyAmenities)
            .Include(p => p.Bookings)
            .FirstOrDefaultAsync(p => p.Id == propertyId, ct);
    }

    public async Task AddBooking(Booking booking)
    {
       await _context.Bookings.AddAsync(booking);
        
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}