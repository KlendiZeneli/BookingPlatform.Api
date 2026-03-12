using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.CreateProperty;

public class CreatePropertyHandler
    : IRequestHandler<CreatePropertyCommand, Result<CreatePropertyResponse>>
{
    private readonly IPropertyRepository _properties;

    private static readonly IReadOnlyDictionary<AmenitiesEnum, Guid> AmenityGuidMap = new Dictionary<AmenitiesEnum, Guid>
    {
        { AmenitiesEnum.WiFi, new Guid("00000000-0000-0000-0000-000000000001") },
        { AmenitiesEnum.AirConditioning, new Guid("00000000-0000-0000-0000-000000000002") },
        { AmenitiesEnum.Parking, new Guid("00000000-0000-0000-0000-000000000003") },
        { AmenitiesEnum.Kitchen, new Guid("00000000-0000-0000-0000-000000000004") },
        { AmenitiesEnum.TV, new Guid("00000000-0000-0000-0000-000000000005") },
        { AmenitiesEnum.WashingMachine, new Guid("00000000-0000-0000-0000-000000000006") },
        { AmenitiesEnum.Balcony, new Guid("00000000-0000-0000-0000-000000000007") },
        { AmenitiesEnum.SeaView, new Guid("00000000-0000-0000-0000-000000000008") },
        { AmenitiesEnum.Pool, new Guid("00000000-0000-0000-0000-000000000009") },
        { AmenitiesEnum.Breakfast, new Guid("00000000-0000-0000-0000-00000000000a") },
        { AmenitiesEnum.PetsAllowed, new Guid("00000000-0000-0000-0000-00000000000b") }
    };

    public CreatePropertyHandler(IPropertyRepository properties)
    {
        _properties = properties;
    }

    public async Task<Result<CreatePropertyResponse>> Handle(
        CreatePropertyCommand request,
        CancellationToken ct)
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            OwnerProfileId = request.OwnerProfileId,
            Name = request.Name,
            Description = request.Description,
            PropertyType = request.PropertyType,
            // Address will be created and attached below
            MaxGuests = request.MaxGuests,
            Bedrooms = request.Bedrooms,
            Beds = request.Beds,
            Bathrooms = request.Bathrooms,
            PricePerNight = request.PricePerNight,
            CheckInTime = request.CheckInTime,
            CheckOutTime = request.CheckOutTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // create address entity from DTO and attach to property
        var address = new BookingPlatform.Domain.Entities.Address
        {
            Id = Guid.NewGuid(),
            Country = request.Address.Country,
            City = request.Address.City,
            Street = request.Address.Street,
            PostalCode = request.Address.PostalCode,
            CreatedAt = DateTime.UtcNow
        };

        property.Address = address;

        foreach (var amenityName in request.Amenities ?? Enumerable.Empty<string>())
        {
            if (!Enum.TryParse<AmenitiesEnum>(amenityName, true, out var amenityEnum))
                return Errors.InvalidAmenity;

            if (!AmenityGuidMap.TryGetValue(amenityEnum, out var amenityGuid))
                return Errors.InvalidAmenity;

            property.PropertyAmenities.Add(new PropertyAmenity
            {
                PropertyId = property.Id,
                AmenityId = amenityGuid
            });
        }

        foreach (var img in request.Images ?? Enumerable.Empty<CreateImageDto>())
        {
            property.Images.Add(new BookingPlatform.Domain.Entities.PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyId = property.Id,
                ImageData = Convert.FromBase64String(img.Base64Data),
                ContentType = img.ContentType,
                IsPrimary = img.IsPrimary
            });
        }

        // ensure at most one primary image
        var primaryImages = property.Images.Where(i => i.IsPrimary).ToList();
        if (primaryImages.Count > 1)
            primaryImages.Skip(1).ToList().ForEach(i => i.IsPrimary = false);

        await _properties.AddAsync(property, ct);
        await _properties.SaveChangesAsync(ct);

        return new CreatePropertyResponse(property.Id);
    }
}