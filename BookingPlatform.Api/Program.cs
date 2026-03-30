using Application.Behaviors;
using BookingPlatform.Api.Endpoints.Properties;
using BookingPlatform.Api.Endpoints.Reviews;
using BookingPlatform.Api.Middleware;
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
using System.Reflection.Metadata;
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

            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
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

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<INotificationService, BookingPlatform.Api.Services.SignalRNotificationService>();

// Kafka configuration
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IEventProducer, KafkaProducer>();


var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// SignalR
app.MapHub<BookingPlatform.Api.Hubs.NotificationHub>("/hubs/notifications");

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
app.MapEndpoints();

app.Run();

