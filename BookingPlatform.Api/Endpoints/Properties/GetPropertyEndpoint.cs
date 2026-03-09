using BookingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Properties;

public static class GetPropertyEndpoint
{
    public static void MapGetPropertyEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{propertyId:guid}", async (
            Guid propertyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new BookingPlatform.Application.Features.Properties.GetProperty.GetPropertyQuery(propertyId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        });
    }
}
