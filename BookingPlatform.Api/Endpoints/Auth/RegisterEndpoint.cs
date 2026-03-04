using BookingPlatform.Application.Common;
using BookingPlatform.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace BookingPlatform.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register",
            async (RegisterCommand command, RegisterHandler handler, CancellationToken ct) =>
            {
                var result = await handler.Handle(command, ct);

                if (!result.IsSuccess)
                {
                    // map error type to proper HTTP result
                    return result.Error.Type switch
                    {
                        ErrorType.NotFound => Results.NotFound(result.Error.Description),
                        ErrorType.Unauthorized => Results.Unauthorized(),
                        ErrorType.Validation => Results.BadRequest(result.Error.Description),
                        _ => Results.BadRequest(result.Error.Description)
                    };
                }

                return Results.Ok(result);
            });
    }
}
