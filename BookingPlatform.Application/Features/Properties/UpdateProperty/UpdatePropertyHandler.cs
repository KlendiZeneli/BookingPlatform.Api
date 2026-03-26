using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.UpdateProperty;

public class UpdatePropertyHandler : IRequestHandler<UpdatePropertyCommand, Result<bool>>
{
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventProducer _events;

    private static readonly IReadOnlyDictionary<AmenitiesEnum, Guid> AmenityGuidMap = new Dictionary<AmenitiesEnum, Guid>
    {
        { AmenitiesEnum.WiFi,            new Guid("00000000-0000-0000-0000-000000000001") },
        { AmenitiesEnum.AirConditioning, new Guid("00000000-0000-0000-0000-000000000002") },
        { AmenitiesEnum.Parking,         new Guid("00000000-0000-0000-0000-000000000003") },
        { AmenitiesEnum.Kitchen,         new Guid("00000000-0000-0000-0000-000000000004") },
        { AmenitiesEnum.TV,              new Guid("00000000-0000-0000-0000-000000000005") },
        { AmenitiesEnum.WashingMachine,  new Guid("00000000-0000-0000-0000-000000000006") },
        { AmenitiesEnum.Balcony,         new Guid("00000000-0000-0000-0000-000000000007") },
        { AmenitiesEnum.SeaView,         new Guid("00000000-0000-0000-0000-000000000008") },
        { AmenitiesEnum.Pool,            new Guid("00000000-0000-0000-0000-000000000009") },
        { AmenitiesEnum.Breakfast,       new Guid("00000000-0000-0000-0000-00000000000a") },
        { AmenitiesEnum.PetsAllowed,     new Guid("00000000-0000-0000-0000-00000000000b") }
    };

    public UpdatePropertyHandler(IPropertyRepository properties, ICurrentUserService currentUser, IEventProducer events)
    {
        _properties = properties;
        _currentUser = currentUser;
        _events = events;
    }

    public async Task<Result<bool>> Handle(UpdatePropertyCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var property = await _properties.GetByIdAsync(request.PropertyId, ct);
        if (property == null) return Errors.PropertyNotFound;

        if (property.OwnerProfileId != userId.Value) return Errors.NotAuthorized;

        property.Name = request.Name;
        property.Description = request.Description;
        property.PropertyType = request.PropertyType;
        property.MaxGuests = request.MaxGuests;
        property.Bedrooms = request.Bedrooms;
        property.Beds = request.Beds;
        property.Bathrooms = request.Bathrooms;
        property.PricePerNight = request.PricePerNight;
        property.CheckInTime = TimeOnly.Parse(request.CheckInTime);
        property.CheckOutTime = TimeOnly.Parse(request.CheckOutTime);
        property.IsActive = request.IsActive;

        // replace amenities via direct DbContext operations to avoid EF state conflicts
        var newAmenities = new List<PropertyAmenity>();
        foreach (var name in request.AmenityNames ?? Enumerable.Empty<string>())
        {
            if (!Enum.TryParse<AmenitiesEnum>(name, true, out var amenityEnum))
                return Errors.InvalidAmenity;

            if (!AmenityGuidMap.TryGetValue(amenityEnum, out var amenityGuid))
                return Errors.InvalidAmenity;

            newAmenities.Add(new PropertyAmenity { PropertyId = property.Id, AmenityId = amenityGuid });
        }
        await _properties.ReplaceAmenitiesAsync(property.Id, newAmenities, ct);

        // replace images if provided
        if (request.Images != null)
        {
            var newImages = request.Images.Select(img => new PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyId = property.Id,
                ImageData = Convert.FromBase64String(img.Base64Data),
                ContentType = img.ContentType,
                IsPrimary = img.IsPrimary
            }).ToList();

            // ensure at most one primary image
            if (newImages.Count(i => i.IsPrimary) > 1)
                newImages.Where(i => i.IsPrimary).Skip(1).ToList().ForEach(i => i.IsPrimary = false);

            await _properties.ReplaceImagesAsync(property.Id, newImages, ct);
        }

        await _properties.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.PropertyUpdated,
            new PropertyUpdatedEvent(property.Id, property.OwnerProfileId, property.Name, property.IsActive, DateTime.UtcNow), ct);

        return true;
    }
}
