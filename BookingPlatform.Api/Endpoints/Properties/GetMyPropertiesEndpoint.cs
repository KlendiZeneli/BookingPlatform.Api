using BookingPlatform.Api.Endpoints;
using BookingPlatform.Application.Features.Properties.GetMyProperties;
using MediatR;

namespace BookingPlatform.Api.Endpoints.Properties;

public class GetMyPropertiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/my", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetMyPropertiesQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(result.Error!.Description, statusCode: result.Error.Code);
        })
        .RequireAuthorization("Owner")
        .WithTags("Properties")
        .WithSummary("Get owner's own properties with booking counts");
    }
}
