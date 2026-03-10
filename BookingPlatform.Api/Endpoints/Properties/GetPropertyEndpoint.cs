using BookingPlatform.Application.Features.Properties.GetProperty;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Properties;

public class GetPropertyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{propertyId:guid}", async (
            Guid propertyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetPropertyQuery(propertyId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Json(result.Error.Description, statusCode: result.Error.Code);
        }).WithTags("Properties")
        .WithSummary("Get property by ID")
        .AllowAnonymous()
        .Produces<GetPropertyResponse>()
        .ProducesValidationProblem();
    }
}
