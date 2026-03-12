using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Properties.GetAvailability;
using MediatR;
using System;

namespace BookingPlatform.Api.Endpoints.Properties;

public class GetAvailabilityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{propertyId:guid}/availability",
            async (Guid propertyId, DateTime checkIn, DateTime checkOut, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetAvailabilityQuery(propertyId, checkIn, checkOut), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
            })
        .AllowAnonymous()
        .WithTags("Properties")
        .WithSummary("Check if a property is available for a given date range and return all blocked periods");
    }
}
