using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BookingPlatform.Application.Features.Properties.SearchProperties;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IPropertyRepository : ICrudRepository<Property>
{
    Task<(List<Property> Items, int TotalCount)> SearchAsync(
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
        CancellationToken ct
    );
    Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken ct);
    Task AddBooking(Booking booking);
}
