using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Application.Features.Properties.SearchProperties;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Application.Features.Properties.SearchProperties;

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

    public async Task<(List<Property> Items, int TotalCount)> SearchAsync(
        string? country,
        string? city,
        DateTime? checkIn,
        DateTime? checkOut,
        int? guests,
        decimal? minPrice,
        decimal? maxPrice,
        PropertyTypeEnum? propertyType,
        int? minBedrooms,
        int? minBathrooms,
        AmenitiesEnum[]? amenities,
        double? minRating,
        SortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _context.Properties
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.PropertyAmenities)
                .ThenInclude(pa => pa.Amenity)
            .Include(p => p.Bookings)
            .Where(p => p.IsActive)
            .AsQueryable();

        // ── Location ─────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(p => p.Address.Country.ToLower().Contains(country.ToLower()));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.Address.City.ToLower().Contains(city.ToLower()));

        // ── Availability — exclude properties with overlapping confirmed bookings
        if (checkIn.HasValue && checkOut.HasValue)
            query = query.Where(p => !p.Bookings.Any(b =>
                (b.BookingStatus == BookingPlatform.Domain.Enums.BookingStatus.Confirmed ||
                 b.BookingStatus == BookingPlatform.Domain.Enums.BookingStatus.Created) &&
                b.StartDate < checkOut.Value &&
                b.EndDate > checkIn.Value));

        // ── Guests ────────────────────────────────────────────────────────────
        if (guests.HasValue)
            query = query.Where(p => p.MaxGuests >= guests.Value);

        // ── Price ─────────────────────────────────────────────────────────────
        if (minPrice.HasValue)
            query = query.Where(p => p.PricePerNight >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.PricePerNight <= maxPrice.Value);

        // ── Property type ─────────────────────────────────────────────────────
        if (propertyType.HasValue)
            query = query.Where(p => p.PropertyType == propertyType.Value);

        // ── Rooms ─────────────────────────────────────────────────────────────
        if (minBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms >= minBedrooms.Value);

        if (minBathrooms.HasValue)
            query = query.Where(p => p.Bathrooms >= minBathrooms.Value);

        // ── Amenities — property must have ALL requested amenities ────────────
        if (amenities != null && amenities.Any())
            foreach (var amenity in amenities)
                query = query.Where(p => p.PropertyAmenities.Any(pa => pa.Amenity.Name == amenity));

        // ── Rating ────────────────────────────────────────────────────────────
        if (minRating.HasValue)
            query = query.Where(p => p.AverageRating >= minRating.Value);

        // ── Total count before pagination ─────────────────────────────────────
        var totalCount = await query.CountAsync(ct);

        // ── Sorting ───────────────────────────────────────────────────────────
        query = sortBy switch
        {
            SortBy.PriceAsc => query.OrderBy(p => p.PricePerNight),
            SortBy.PriceDesc => query.OrderByDescending(p => p.PricePerNight),
            SortBy.Rating => query.OrderByDescending(p => p.AverageRating),
            SortBy.Newest => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.ReviewCount) // Relevance
        };

        // ── Pagination ────────────────────────────────────────────────────────
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}