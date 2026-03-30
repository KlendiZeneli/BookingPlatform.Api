using BookingPlatform.Application.Features.Reviews.GetPropertyReviews;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace BookingPlatform.Api.Endpoints.Reviews;

public class GetPropertyReviewsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{propertyId:guid}/reviews", async (
            Guid propertyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetPropertyReviewsQuery(propertyId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        });
    }
}
