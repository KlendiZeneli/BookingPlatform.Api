using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Enums;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.GetMyProperties;

public class GetMyPropertiesHandler : IRequestHandler<GetMyPropertiesQuery, Result<GetMyPropertiesResponse>>
{
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;

    public GetMyPropertiesHandler(IPropertyRepository properties, ICurrentUserService currentUser)
    {
        _properties = properties;
        _currentUser = currentUser;
    }

    public async Task<Result<GetMyPropertiesResponse>> Handle(GetMyPropertiesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return Errors.NotAuthenticated;

        var props = await _properties.GetByOwnerIdAsync(userId.Value, ct);

        var dtos = props.Select(p => new MyPropertyDto(
            p.Id,
            p.Name,
            p.Description,
            p.PropertyType.ToString(),
            p.Address?.City ?? string.Empty,
            p.Address?.Country ?? string.Empty,
            p.PricePerNight,
            p.MaxGuests,
            p.Bedrooms,
            p.Beds,
            p.Bathrooms,
            p.AverageRating,
            p.ReviewCount,
            p.IsActive,
            p.Bookings?.Count(b => b.BookingStatus == BookingStatus.Created) ?? 0,
            p.Bookings?.Count(b => b.BookingStatus == BookingStatus.Confirmed) ?? 0
        ));

        return new GetMyPropertiesResponse(dtos);
    }
}
