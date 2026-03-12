namespace BookingPlatform.Application.Features.Properties.CreateProperty;

using BookingPlatform.Application.Common;
using BookingPlatform.Domain.Enums;
using MediatR;

public record CreatePropertyCommand(
    Guid OwnerProfileId,
    string Name,
    string Description,
    PropertyTypeEnum PropertyType,
    CreateAddressDto Address,
    int MaxGuests,
    int Bedrooms,
    int Beds,
    int Bathrooms,
    decimal PricePerNight,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime,
    List<string> Amenities,
    List<CreateImageDto>? Images
) : IRequest<Result<CreatePropertyResponse>>;

public record CreateAddressDto(string Country, string City, string Street, string PostalCode);
public record CreateImageDto(string Base64Data, string ContentType, bool IsPrimary);
