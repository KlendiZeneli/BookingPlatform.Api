using Microsoft.AspNetCore.SignalR;

namespace BookingPlatform.Api.Hubs;

public class NotificationHub : Hub
{
    public async Task SendToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public override Task OnConnectedAsync()
    {
        // map connection to user via query string or claims in production
        return base.OnConnectedAsync();
    }
}
