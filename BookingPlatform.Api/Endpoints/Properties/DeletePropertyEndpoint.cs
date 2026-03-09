using BookingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Properties;

public static class DeletePropertyEndpoint
{
    public static void MapDeletePropertyEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/properties/{propertyId:guid}", async (
            Guid propertyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new BookingPlatform.Application.Features.Properties.DeleteProperty.DeletePropertyCommand(propertyId), ct);
            return result.IsSuccess ? Results.Ok() : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        }).RequireAuthorization("Owner");
    }
}
