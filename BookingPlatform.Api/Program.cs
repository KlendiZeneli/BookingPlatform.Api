using Application.Behaviors;
using BookingPlatform.Api.Endpoints.Properties;
using BookingPlatform.Api.Endpoints.Reviews;
using BookingPlatform.Api.Hubs;
using BookingPlatform.Api.Middleware;
using BookingPlatform.Api.Services;
using BookingPlatform.API.Endpoints.Auth;
using BookingPlatform.API.Endpoints.OwnerProfiles;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Application.Features;
using BookingPlatform.Application.Features.Auth.Login;
using BookingPlatform.Application.Features.Auth.Register;
using BookingPlatform.Api.Endpoints.Bookings;
using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Infrastructure.Persistence.Repositories;
using BookingPlatform.Infrastructure.Repositories;
using BookingPlatform.Infrastructure.Services;
using BookingPlatform.Infrastructure.Kafka;
using DotNetEnv;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using BookingPlatform.API.Extensions;


var builder = WebApplication.CreateBuilder(args);

Env.Load();
var connectionString = Environment.GetEnvironmentVariable("BOOKING_DB")!;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.AddSignalR();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HandlerMarker>());
builder.Services.AddValidatorsFromAssemblyContaining<HandlerMarker>();

ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IOwnerProfileRepository, OwnerProfileRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<IReviewRepository, BookingPlatform.Infrastructure.Persistence.Repositories.ReviewRepository>();
builder.Services.AddScoped<IBookingRepository, BookingPlatform.Infrastructure.Persistence.Repositories.BookingRepository>();
// (email service registration above)

var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT"));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(key),

            // Use the NameIdentifier claim as the user identifier for SignalR groups
            NameClaimType = ClaimTypes.NameIdentifier
        };

        // SignalR sends the JWT via query string for WebSocket connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Guest", policy =>
        policy.RequireRole("Guest"));

    options.AddPolicy("Owner", policy =>
        policy.RequireRole("Owner"));
});

builder.Services.AddEndpoints();

// Kafka configuration
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IEventProducer, KafkaProducer>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();


var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseGlobalExceptionHandler();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();


//app.MapGet("/", () => "Hello World!");

app.MapRegisterEndpoint();
app.MapLoginEndpoint();
app.MapVerifyOwnerProfileEndpoint();
app.MapCreateOwnerProfileEndpoint();
app.MapMakeBookingEndpoint();
app.MapMakeReviewEndpoint();
app.MapPasswordEndpoints();
app.MapVerifyBookingEndpoint();
app.MapCancelBookingEndpoint();
app.MapGetPropertyReviewsEndpoint();
app.MapEndpoints();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

