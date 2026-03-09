using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

    public Task<IEnumerable<Property>> GetAllAsync(CancellationToken ct)
        => _context.Properties
            .Include(p => p.PropertyAmenities)
            .Include(p => p.Bookings)
            .ToListAsync(ct).ContinueWith(t => (IEnumerable<Property>)t.Result, ct);

    public Task UpdateAsync(Property entity, CancellationToken ct)
    {
        _context.Properties.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var p = await _context.Properties.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p != null) _context.Properties.Remove(p);
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
