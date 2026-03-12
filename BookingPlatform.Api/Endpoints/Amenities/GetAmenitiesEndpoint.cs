using BookingPlatform.Api.Endpoints;
using BookingPlatform.Domain.Enums;

namespace BookingPlatform.Api.Endpoints.Amenities;

public class GetAmenitiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/amenities", () =>
        {
            var names = Enum.GetNames<AmenitiesEnum>();
            return Results.Ok(names);
        })
        .AllowAnonymous()
        .WithTags("Amenities")
        .WithSummary("List all valid amenity names");
    }
}
