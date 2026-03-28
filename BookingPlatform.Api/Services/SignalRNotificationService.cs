using BookingPlatform.Api.Hubs;
using BookingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BookingPlatform.Api.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(string userId, string method, object payload, CancellationToken ct = default)
    {
        await _hubContext.Clients.User(userId).SendAsync(method, payload, ct);
    }
}
