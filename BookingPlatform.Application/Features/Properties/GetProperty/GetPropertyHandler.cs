using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Properties.GetProperty;

public class GetPropertyHandler : IRequestHandler<GetPropertyQuery, Result<GetPropertyResponse>>
{
    private readonly IPropertyRepository _properties;

    public GetPropertyHandler(IPropertyRepository properties)
    {
        _properties = properties;
    }

    public async Task<Result<GetPropertyResponse>> Handle(GetPropertyQuery request, CancellationToken ct)
    {
        var prop = await _properties.GetByIdAsync(request.PropertyId, ct);
        if (prop == null) return Errors.PropertyNotFound;

        // Map to DTO to avoid circular references during JSON serialization
        var bookings = prop.Bookings?.Select(b => new BookingDto(
            b.Id,
            b.PropertyId,
            b.GuestId,
            b.StartDate,
            b.EndDate,
            b.GuestCount,
            b.TotalPrice,
            b.BookingStatus.ToString(),
            b.CreatedOnUtc,
            b.ConfirmedOnUtc,
            b.RejectedOnUtc,
            b.CompletedOnUtc,
            b.CancelledOnUtc
        )) ?? Enumerable.Empty<BookingDto>();

        AddressDto? address = null;
        if (prop.Address != null)
        {
            address = new AddressDto(prop.Address.Country, prop.Address.City, prop.Address.Street, prop.Address.PostalCode);
        }

        var dto = new PropertyDto(
            prop.Id,
            prop.OwnerProfileId,
            prop.Name,
            prop.Description,
            (int)prop.PropertyType,
            prop.AddressId,
            address,
            prop.MaxGuests,
            prop.Bedrooms,
            prop.Beds,
            prop.Bathrooms,
            prop.PricePerNight,
            prop.CheckInTime.ToString(),
            prop.CheckOutTime.ToString(),
            prop.IsActive,
            prop.ReviewCount,
            prop.AverageRating,
            prop.LastBookedOnUtc,
            bookings
        );

        return new GetPropertyResponse(dto);
    }
}
