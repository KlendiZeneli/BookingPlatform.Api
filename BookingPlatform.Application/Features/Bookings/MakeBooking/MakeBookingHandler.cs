using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Bookings.MakeBooking;

public class MakeBookingHandler : IRequestHandler<MakeBookingCommand, Result<MakeBookingResponse>>
{
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventProducer _events;

    public MakeBookingHandler(IPropertyRepository properties, ICurrentUserService currentUser, IEventProducer events)
    {
        _properties = properties;
        _currentUser = currentUser;
        _events = events;
    }

    public async Task<Result<MakeBookingResponse>> Handle(MakeBookingCommand request, CancellationToken ct)
    {
        var property = await _properties.GetByIdAsync(request.PropertyId, ct);
        if (property == null) return Errors.PropertyNotFound;

        // normalize dates to date-only ranges
        var start = request.StartDate.Date;
        var end = request.EndDate.Date;

        if (start >= end) return Errors.FieldsRequired;
        if (request.GuestCount > property.MaxGuests) return Errors.TooManyGuests;

        // check overlapping bookings: any booking where (start < existing.EndDate) && (end > existing.StartDate)
        var overlap = property.Bookings.Any(b => b.BookingStatus != BookingPlatform.Domain.Enums.BookingStatus.Cancelled && start < b.EndDate.Date && end > b.StartDate.Date);
        if (overlap) return Errors.BookingConflict;

        var nights = (end - start).Days;
        var priceForPeriod = property.PricePerNight * nights;

        // ensure user is authenticated
        var currentUserId = _currentUser.UserId;
        if (currentUserId == null)
            return Errors.NotAuthenticated;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            PropertyId = property.Id,
            GuestId = currentUserId.Value,
            StartDate = start,
            EndDate = end,
            GuestCount = request.GuestCount,
            CleaningFee = 0m,
            AmenitiesUpCharge = 0m,
            PriceForPeriod = priceForPeriod,
            TotalPrice = priceForPeriod,
            BookingStatus = BookingPlatform.Domain.Enums.BookingStatus.Created,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _properties.AddBooking(booking);

        await _properties.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.BookingCreated,
            new BookingCreatedEvent(booking.Id, booking.PropertyId, booking.GuestId, booking.StartDate, booking.EndDate, booking.GuestCount, booking.TotalPrice, DateTime.UtcNow), ct);

        return new MakeBookingResponse(booking.Id);
    }
}
