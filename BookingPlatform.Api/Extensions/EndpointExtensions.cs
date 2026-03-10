using BookingPlatform.Api.Endpoints;
using MediatR;

namespace BookingPlatform.API.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                        && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in endpointTypes)
            services.AddSingleton(typeof(IEndpoint), type);

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(app);

        return app;
    }
}