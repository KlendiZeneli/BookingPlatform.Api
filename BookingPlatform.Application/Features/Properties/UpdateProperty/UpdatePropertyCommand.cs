using MediatR;
using BookingPlatform.Application.Common;
using BookingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Properties.UpdateProperty;

public record UpdatePropertyCommand(
    Guid PropertyId,
    string Name,
    string Description,
    PropertyTypeEnum PropertyType,
    int MaxGuests,
    int Bedrooms,
    int Beds,
    int Bathrooms,
    decimal PricePerNight,
    string CheckInTime,
    string CheckOutTime,
    bool IsActive,
    List<string> AmenityNames,
    List<UpdateImageDto>? Images
) : IRequest<Result<bool>>;

public record UpdateImageDto(string Base64Data, string ContentType, bool IsPrimary);
