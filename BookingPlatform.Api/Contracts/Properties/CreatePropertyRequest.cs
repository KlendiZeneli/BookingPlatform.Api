using BookingPlatform.Domain.Enums;

namespace BookingPlatform.Api.Contracts.Properties;

public record CreatePropertyRequest(
    string Name,
    string Description,
    PropertyTypeEnum PropertyType,
    CreateAddressRequest Address,
    int MaxGuests,
    int Bedrooms,
    int Beds,
    int Bathrooms,
    decimal PricePerNight,
    string CheckInTime,
    string CheckOutTime,
    List<string> AmenityNames,
    List<CreateImageRequest>? Images
);

public record CreateAddressRequest(string Country, string City, string Street, string PostalCode);
public record CreateImageRequest(string Base64Data, string ContentType, bool IsPrimary);
