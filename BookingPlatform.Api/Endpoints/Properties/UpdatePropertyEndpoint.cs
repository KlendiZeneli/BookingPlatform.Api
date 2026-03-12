using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Properties.UpdateProperty;
using MediatR;
using System;

namespace BookingPlatform.Api.Endpoints.Properties;

public class UpdatePropertyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/properties/{propertyId:guid}", async (
            Guid propertyId,
            UpdatePropertyCommand cmd,
            IMediator mediator,
            CancellationToken ct) =>
        {
            // ensure route id matches body id
            var command = cmd with { PropertyId = propertyId };
            var result = await mediator.Send(command, ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization("Owner")
        .WithTags("Properties")
        .WithSummary("Update a property — owner only");
    }
}
