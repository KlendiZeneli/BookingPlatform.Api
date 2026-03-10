using MediatR;
using BookingPlatform.Application.Common;
using BookingPlatform.Domain.Enums;

namespace BookingPlatform.Application.Features.Properties.SearchProperties;

public record SearchPropertiesCommand(
    // Location
    string? Country,
    string? City,

    // Dates
    DateTime? CheckIn,
    DateTime? CheckOut,

    // Guests
    int? Guests,

    // Price
    decimal? MinPrice,
    decimal? MaxPrice,

    // Property type
    PropertyTypeEnum? PropertyType,

    // Rooms
    int? MinBedrooms,
    int? MinBathrooms,

    // Amenities
    AmenitiesEnum[]? Amenities,

    // Rating
    double? MinRating,

    // Pagination
    int Page = 1,
    int PageSize = 10,

    // Sorting
    SortBy SortBy = SortBy.Relevance

) : IRequest<Result<PagedResponse<PropertyResponse>>>;

public enum SortBy
{
    Relevance,
    PriceAsc,
    PriceDesc,
    Rating,
    Newest
}