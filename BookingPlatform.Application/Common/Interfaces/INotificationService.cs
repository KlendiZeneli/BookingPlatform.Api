namespace BookingPlatform.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendToUserAsync(string userId, string method, object payload, CancellationToken ct = default);
}
