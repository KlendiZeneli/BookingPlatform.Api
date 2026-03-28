using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookingPlatform.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
