using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Enums;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.GetAvailability;

public class GetAvailabilityHandler : IRequestHandler<GetAvailabilityQuery, Result<GetAvailabilityResponse>>
{
    private readonly IPropertyRepository _properties;

    public GetAvailabilityHandler(IPropertyRepository properties)
    {
        _properties = properties;
    }

    public async Task<Result<GetAvailabilityResponse>> Handle(GetAvailabilityQuery request, CancellationToken ct)
    {
        var property = await _properties.GetByIdAsync(request.PropertyId, ct);
        if (property == null) return Errors.PropertyNotFound;

        var activeStatuses = new[] { BookingStatus.Created, BookingStatus.Confirmed };

        // all blocked periods (active bookings) within a reasonable window
        var blocked = (property.Bookings ?? [])
            .Where(b => activeStatuses.Contains(b.BookingStatus))
            .Select(b => new BlockedPeriod(b.StartDate.Date, b.EndDate.Date))
            .ToList();

        var requestedStart = request.CheckIn.Date;
        var requestedEnd = request.CheckOut.Date;

        var isAvailable = !blocked.Any(b => requestedStart < b.EndDate && requestedEnd > b.StartDate);

        return new GetAvailabilityResponse(isAvailable, blocked);
    }
}
