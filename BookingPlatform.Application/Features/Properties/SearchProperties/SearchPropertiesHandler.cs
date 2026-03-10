using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.SearchProperties;

public class SearchPropertiesHandler : IRequestHandler<SearchPropertiesCommand, Result<PagedResponse<PropertyResponse>>>
{
    private readonly IPropertyRepository _properties;

    public SearchPropertiesHandler(IPropertyRepository properties)
    {
        _properties = properties;
    }

    public async Task<Result<PagedResponse<PropertyResponse>>> Handle(SearchPropertiesCommand command, CancellationToken ct)
    {
        (List < Property > items, int totalCount) = await _properties.SearchAsync(
            country: command.Country,
            city: command.City,
            checkIn: command.CheckIn,
            checkOut: command.CheckOut,
            guests: command.Guests,
            minPrice: command.MinPrice,
            maxPrice: command.MaxPrice,
            propertyType: command.PropertyType,
            minBedrooms: command.MinBedrooms,
            minBathrooms: command.MinBathrooms,
            amenities: command.Amenities,
            minRating: command.MinRating,
            sortBy: command.SortBy,
            page: command.Page,
            pageSize: command.PageSize,
            ct: ct
        );

        var responses = items.Select(p => new PropertyResponse(
            Id: p.Id,
            Name: p.Name,
            City: p.Address.City,
            Country: p.Address.Country,
            PricePerNight: p.PricePerNight,
            MaxGuests: p.MaxGuests,
            Bedrooms: p.Bedrooms,
            Bathrooms: p.Bathrooms,
            AverageRating: p.AverageRating,
            ReviewCount: p.ReviewCount,
            PrimaryImageUrl: p.Images.FirstOrDefault(i => i.IsPrimary)?.Url,
            PropertyType: p.PropertyType.ToString()
        )).ToList();

        return new PagedResponse<PropertyResponse>(responses, totalCount, command.Page, command.PageSize);
    }
}