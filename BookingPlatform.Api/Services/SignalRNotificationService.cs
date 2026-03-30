using BookingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using BookingPlatform.Api.Hubs;

namespace BookingPlatform.Api.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;
    public SignalRNotificationService(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task NotifyUserAsync(Guid userId, string message)
    {
        return _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
    }
}
