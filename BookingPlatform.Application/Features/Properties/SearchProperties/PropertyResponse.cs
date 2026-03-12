namespace BookingPlatform.Application.Features.Properties.SearchProperties;

public record PropertyResponse(
    Guid Id,
    string Name,
    string City,
    string Country,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms,
    int Bathrooms,
    double AverageRating,
    int ReviewCount,
    string? PrimaryImageBase64,
    string? PrimaryImageContentType,
    string PropertyType
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}