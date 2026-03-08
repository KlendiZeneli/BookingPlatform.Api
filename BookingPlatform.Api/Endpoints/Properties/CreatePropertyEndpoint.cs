using MediatR;
using System.Security.Claims;
using BookingPlatform.Api.Contracts.Properties;
using BookingPlatform.Application.Features.Properties.CreateProperty;

namespace BookingPlatform.Api.Endpoints.Properties;

public static class CreatePropertyEndpoint
{
    public static void MapCreatePropertyEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/properties", async (
            CreatePropertyRequest request,
            IMediator mediator,
            HttpContext http,
            CancellationToken ct) =>
        {
            var user = http.User;
            var ownerIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim))
                return Results.Unauthorized();

            var ownerId = Guid.Parse(ownerIdClaim);
            var checkIn = TimeOnly.Parse(request.CheckInTime);
            var checkOut = TimeOnly.Parse(request.CheckOutTime);
            var command = new CreatePropertyCommand(
                ownerId,
                request.Name,
                request.Description,
                request.PropertyType,
                new CreateAddressDto(request.Address.Country, request.Address.City, request.Address.Street, request.Address.PostalCode),
                request.MaxGuests,
                request.Bedrooms,
                request.Beds,
                request.Bathrooms,
                request.PricePerNight,
                checkIn,
                checkOut,
                request.AmenityNames
            );

            var result = await mediator.Send(command, ct);

            if (!result.IsSuccess)
                return Results.BadRequest(result.Error.Description);

            return Results.Created(
                $"/api/properties/{result.Value.PropertyId}",
                result.Value);
        })
        .RequireAuthorization("Owner");
    }
}