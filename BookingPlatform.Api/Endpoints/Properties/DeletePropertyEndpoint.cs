using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Application.Features.Properties.GetProperty;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace BookingPlatform.Api.Endpoints.Properties;

public class DeletePropertyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/properties/{propertyId:guid}", async (
            Guid propertyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new BookingPlatform.Application.Features.Properties.DeleteProperty.DeletePropertyCommand(propertyId), ct);
                return result.IsSuccess ? Results.NoContent() : Results.Json(result.Error.Description, statusCode: result.Error.Code);
            })
            .RequireAuthorization("Owner")
            .WithTags("Properties")
            .WithSummary("Delete property by ID — owner only");
    }
}
