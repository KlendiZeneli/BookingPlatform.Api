using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string message);
}
